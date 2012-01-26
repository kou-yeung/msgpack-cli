#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2010 FUJIWARA, Yusuke
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
#endregion -- License Terms --

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using NLiblet.Reflection;

namespace MsgPack.Serialization
{

	// Reduce JIT
	/// <summary>
	///		Defines non-generic functions of <see cref="EmittingSerializerBuilder{T}"/>.
	/// </summary>
	internal static class EmittingSerializerBuilderLogics
	{
		private static readonly Type[] _delegateConstructorParameterTypes = new Type[] { typeof( object ), typeof( IntPtr ) };

		#region -- Arrays --

		public static SerializerEmitter CreateArraySerializerCore( Type targetType, EmitterFlavor emitterFlavor )
		{
			Contract.Requires( targetType != null );

			var emitter = SerializationMethodGeneratorManager.Get().CreateEmitter( targetType, emitterFlavor );
			var traits = targetType.GetCollectionTraits();
			CreatePackArrayProceduresCore( targetType, emitter, traits );
			CreateUnpackArrayProceduresCore( targetType, emitter, traits );
			return emitter;
		}

		private static void CreatePackArrayProceduresCore( Type targetType, SerializerEmitter emitter, CollectionTraits traits )
		{
			var il = emitter.GetPackToMethodILGenerator();
			try
			{
				// Array
				if ( targetType.IsArray )
				{
					/*
					 * // array
					 *  packer.PackArrayHeader( length );
					 * for( int i = 0; i < length; i++ )
					 * {
					 * 		this._serializer.PackTo( packer, collection[ i ] );
					 * }
					 */
					var length = il.DeclareLocal( typeof( int ), "length" );
					il.EmitAnyLdarg( 2 );
					il.EmitLdlen();
					il.EmitAnyStloc( length );
					il.EmitAnyLdarg( 1 );
					il.EmitAnyLdloc( length );
					il.EmitAnyCall( Metadata._Packer.PackArrayHeader );
					il.EmitPop();
					Emittion.EmitFor(
						il,
						length,
						( il0, i ) =>
						{
							Emittion.EmitSerializeValue(
								emitter,
								il0,
								1,
								traits.ElementType,
								il1 =>
								{
									il1.EmitAnyLdarg( 2 );
									il1.EmitAnyLdloc( i );
									il1.EmitLdelem( traits.ElementType );
								}
							);
						}
					);
				}
				else if ( traits.CountProperty == null )
				{
					/*
					 *  array = collection.ToArray();
					 *  packer.PackArrayHeader( length );
					 * for( int i = 0; i < length; i++ )
					 * {
					 * 		this._serializer.PackTo( packer, array[ i ] );
					 * }
					 */
					var array = il.DeclareLocal( traits.ElementType.MakeArrayType(), "array" );
					EmitLoadTarget( targetType, il, 2 );
					il.EmitAnyCall( Metadata._Enumerable.ToArray1Method.MakeGenericMethod( traits.ElementType ) );
					il.EmitAnyStloc( array );
					var length = il.DeclareLocal( typeof( int ), "length" );
					il.EmitAnyLdloc( array );
					il.EmitLdlen();
					il.EmitAnyStloc( length );
					il.EmitAnyLdarg( 1 );
					il.EmitAnyLdloc( length );
					il.EmitAnyCall( Metadata._Packer.PackArrayHeader );
					il.EmitPop();
					Emittion.EmitFor(
						il,
						length,
						( il0, i ) =>
						{
							Emittion.EmitSerializeValue(
								emitter,
								il0,
								1,
								traits.ElementType,
								il1 =>
								{
									il1.EmitAnyLdloc( array );
									il1.EmitAnyLdloc( i );
									il1.EmitLdelem( traits.ElementType );
								}
							);
						}
					);
				}
				else
				{
					/*
					 * // Enumerable
					 *  packer.PackArrayHeader( collection.Count );
					 * foreach( var item in list )
					 * {
					 * 		this._serializer.PackTo( packer, array[ i ] );
					 * }
					 */
					var collection = il.DeclareLocal( targetType, "collection" );
					// This instruction always ldarg, not to be ldarga
					il.EmitAnyLdarg( 2 );
					il.EmitAnyStloc( collection );
					var count = il.DeclareLocal( typeof( int ), "count" );
					EmitLoadTarget( targetType, il, 2 );
					il.EmitGetProperty( traits.CountProperty );
					il.EmitAnyStloc( count );
					il.EmitAnyLdarg( 1 );
					il.EmitAnyLdloc( count );
					il.EmitAnyCall( Metadata._Packer.PackArrayHeader );
					il.EmitPop();
					Emittion.EmitForEach(
						il,
						traits,
						collection,
						( il0, getCurrentEmitter ) =>
						{
							Emittion.EmitSerializeValue(
								emitter,
								il0,
								1,
								traits.ElementType,
								_ => getCurrentEmitter()
							);
						}
					);
				}
				il.EmitRet();
			}
			finally
			{
				il.FlushTrace();
			}
		}

		private static void CreateUnpackArrayProceduresCore( Type targetType, SerializerEmitter emitter, CollectionTraits traits )
		{
			CreateArrayUnpackFrom( targetType, emitter, traits );
			CreateArrayUnpackTo( targetType, emitter, traits );
		}

		private static void CreateArrayUnpackFrom( Type targetType, SerializerEmitter emitter, CollectionTraits traits )
		{
			var il = emitter.GetUnpackFromMethodILGenerator();
			try
			{
				if ( targetType.IsInterface || targetType.IsAbstract )
				{
					il.EmitTypeOf( targetType );
					il.EmitAnyCall( SerializationExceptions.NewNotSupportedBecauseCannotInstanciateAbstractTypeMethod );
					il.EmitThrow();
					return;
				}

				/*
				 *	if (!unpacker.IsArrayHeader)
				 *	{
				 *		throw SerializationExceptions.NewIsNotArrayHeader();
				 *	}
				 *	
				 *	TCollection collection = new ...;
				 *	this.UnpackToCore(unpacker, array);
				 *	return collection;
				 */

				il.EmitAnyLdarg( 1 );
				il.EmitGetProperty( Metadata._Unpacker.IsArrayHeader );
				var endIf = il.DefineLabel( "END_IF" );
				il.EmitBrtrue_S( endIf );
				il.EmitAnyCall( SerializationExceptions.NewIsNotArrayHeaderMethod );
				il.EmitThrow();
				il.MarkLabel( endIf );
				var collection = il.DeclareLocal( targetType, "collection" );
				// Emit newobj, newarr, or call ValueType..ctor()
				Emittion.EmitConstruction(
					il,
					collection,
					il0 => Emittion.EmitGetUnpackerItemsCountAsInt32( il0, 1 )
				);

				EmitInvokeArrayUnpackToHelper( targetType, emitter, traits, il, 1, il0 => il0.EmitAnyLdloc( collection ) );

				il.EmitAnyLdloc( collection );
				il.EmitRet();
			}
			finally
			{
				il.FlushTrace();
			}
		}

		private static void CreateArrayUnpackTo( Type targetType, SerializerEmitter emitter, CollectionTraits traits )
		{
			var il = emitter.GetUnpackToMethodILGenerator();
			try
			{
				EmitInvokeArrayUnpackToHelper( targetType, emitter, traits, il, 1, il0 => il0.EmitAnyLdarg( 2 ) );
				il.EmitRet();
			}
			finally
			{
				il.FlushTrace();
			}
		}

		private static void EmitInvokeArrayUnpackToHelper( Type targetType, SerializerEmitter emitter, CollectionTraits traits, TracingILGenerator il, int unpackerArgumentIndex, Action<TracingILGenerator> loadCollectionEmitting )
		{
			il.EmitAnyLdarg( unpackerArgumentIndex );
			var serializerGetting = emitter.RegisterSerializer( traits.ElementType );

			if ( targetType.IsArray )
			{
				// Array
				/*
				 * UnpackHelpers.UnpackArrayTo( unpacker, GET_SERIALIZER, collection );
				 */
				serializerGetting( il, 0 );
				loadCollectionEmitting( il );
				il.EmitAnyCall( Metadata._UnpackHelpers.UnpackArrayTo_1.MakeGenericMethod( traits.ElementType ) );
			}
			else if ( targetType.IsGenericType )
			{
				serializerGetting( il, 0 );
				loadCollectionEmitting( il );
				if ( targetType.IsValueType )
				{
					il.EmitBox( targetType );
				}

				if ( traits.AddMethod.ReturnType == null || traits.AddMethod.ReturnType == typeof( void ) )
				{
					// with void Add( T item )
					/*
					 * Action<T> addition = TCollection.Add
					 * UnpackHelpers.UnpackCollectionTo( unpacker, GET_SERIALIZER, collection, addition );
					 */
					var itemType = traits.AddMethod.GetParameters()[ 0 ].ParameterType;
					EmitNewDelegate( il, targetType, traits.AddMethod, loadCollectionEmitting, typeof( Action<> ).MakeGenericType( itemType ) );
					il.EmitAnyCall( Metadata._UnpackHelpers.UnpackCollectionTo_1.MakeGenericMethod( itemType ) );
				}
				else
				{
					// with TDiscarded Add( T item )
					/*
					 * Func<T, TDiscarded> addition = TCollection.Add
					 * UnpackHelpers.UnpackCollectionTo( unpacker, GET_SERIALIZER, collection, addition );
					 */
					var itemType = traits.AddMethod.GetParameters()[ 0 ].ParameterType;
					var discardingType = traits.AddMethod.ReturnType;
					EmitNewDelegate( il, targetType, traits.AddMethod, loadCollectionEmitting, typeof( Func<,> ).MakeGenericType( itemType, discardingType ) );
					il.EmitAnyCall( Metadata._UnpackHelpers.UnpackCollectionTo_2.MakeGenericMethod( itemType, discardingType ) );
				}
			}
			else
			{
				loadCollectionEmitting( il );
				if ( targetType.IsValueType )
				{
					il.EmitBox( targetType );
				}

				if ( traits.AddMethod.ReturnType == null || traits.AddMethod.ReturnType == typeof( void ) )
				{
					// with void Add( object item )
					/*
					 * Action<object> addition = TCollection.Add
					 * UnpackHelpers.UnpackCollectionTo( unpacker, collection, addition );
					 */
					EmitNewDelegate( il, targetType, traits.AddMethod, loadCollectionEmitting, typeof( Action<object> ) );
					il.EmitAnyCall( Metadata._UnpackHelpers.UnpackNonGenericCollectionTo );
				}
				else
				{
					// with TDiscarded Add( object item )
					/*
					 * Func<TDiscarded> addition = TCollection.Add
					 * UnpackHelpers.UnpackCollectionTo( unpacker, collection, addition );
					 */
					var discardingType = traits.AddMethod.ReturnType;
					EmitNewDelegate( il, targetType, traits.AddMethod, loadCollectionEmitting, typeof( Func<,> ).MakeGenericType( typeof( object ), discardingType ) );
					il.EmitAnyCall( Metadata._UnpackHelpers.UnpackNonGenericCollectionTo_1.MakeGenericMethod( discardingType ) );
				}
			}
		}

		private static void EmitNewDelegate( TracingILGenerator il, Type targetType, MethodInfo method, Action<TracingILGenerator> loadTargetEmitting, Type delegateType )
		{
			loadTargetEmitting( il );
			if ( targetType.IsValueType )
			{
				il.EmitBox( targetType );
			}

			if ( method.IsStatic || method.IsFinal || !method.IsVirtual )
			{
				il.EmitLdftn( method );
			}
			else
			{
				il.EmitDup();
				il.EmitLdvirtftn( method );
			}

			il.EmitNewobj( delegateType.GetConstructor( _delegateConstructorParameterTypes ) );
		}

		#endregion -- Arrays --

		#region -- Maps --

		public static SerializerEmitter CreateMapSerializerCore( Type targetType, EmitterFlavor emitterFlavor )
		{
			Contract.Requires( targetType != null );

			var emitter = SerializationMethodGeneratorManager.Get().CreateEmitter( targetType, emitterFlavor );
			var traits = targetType.GetCollectionTraits();
			CreateMapPack(
				targetType,
				emitter,
				traits
			);
			CreateMapUnpack(
				targetType,
				emitter,
				traits
			);

			return emitter;
		}

		private static void CreateMapPack( Type targetType, SerializerEmitter emiter, CollectionTraits traits )
		{
			var il = emiter.GetPackToMethodILGenerator();
			try
			{

				/*
				 * 	int count = ((ICollection<KeyValuePair<string, DateTime>>)dictionary).Count;
				 * 	packer.PackMapHeader(count);
				 * 	foreach (KeyValuePair<string, DateTime> current in dictionary)
				 * 	{
				 * 		this._serializer0.PackTo(packer, current.Key);
				 * 		this._serializer1.PackTo(packer, current.Value);
				 * 	}
				 */

				var collection = il.DeclareLocal( targetType, "collection" );
				var item = il.DeclareLocal( traits.ElementType, "item" );
				var keyProperty = traits.ElementType.GetProperty( "Key" );
				var valueProperty = traits.ElementType.GetProperty( "Value" );
				// This instruction is always ldarg, not to be ldarga.
				il.EmitAnyLdarg( 2 );
				il.EmitAnyStloc( collection );
				var count = il.DeclareLocal( typeof( int ), "count" );
				EmitLoadTarget( targetType, il, collection );
				il.EmitGetProperty( traits.CountProperty );
				il.EmitAnyStloc( count );
				il.EmitAnyLdarg( 1 );
				il.EmitAnyLdloc( count );
				il.EmitAnyCall( Metadata._Packer.PackMapHeader );
				il.EmitPop();

				Emittion.EmitForEach(
					il,
					traits,
					collection,
					( il0, getCurrentEmitter ) =>
					{
						if ( traits.ElementType.IsGenericType )
						{
							Contract.Assert( traits.ElementType.GetGenericTypeDefinition() == typeof( KeyValuePair<,> ) );
							getCurrentEmitter();
							il0.EmitAnyStloc( item );
							Emittion.EmitSerializeValue(
								emiter,
								il0,
								1,
								traits.ElementType.GetGenericArguments()[ 0 ],
								il1 =>
								{
									il1.EmitAnyLdloca( item );
									il1.EmitGetProperty( keyProperty );
								}
							);

							Emittion.EmitSerializeValue(
								emiter,
								il0,
								1,
								traits.ElementType.GetGenericArguments()[ 1 ],
								il1 =>
								{
									il1.EmitAnyLdloca( item );
									il1.EmitGetProperty( valueProperty );
								}
							);
						}
						else
						{
							Contract.Assert( traits.ElementType == typeof( DictionaryEntry ) );
							getCurrentEmitter();
							il0.EmitAnyStloc( item );
							Emittion.EmitSerializeValue(
								emiter,
								il0,
								1,
								typeof( MessagePackObject ),
								il1 =>
								{
									il0.EmitAnyLdloca( item );
									il0.EmitGetProperty( Metadata._DictionaryEntry.Key );
									il0.EmitUnbox_Any( typeof( MessagePackObject ) );
								}
							);

							Emittion.EmitSerializeValue(
								emiter,
								il0,
								1,
								typeof( MessagePackObject ),
								il1 =>
								{
									il0.EmitAnyLdloca( item );
									il0.EmitGetProperty( Metadata._DictionaryEntry.Value );
									il0.EmitUnbox_Any( typeof( MessagePackObject ) );
								}
							);
						}
					}
				);
				il.EmitRet();
			}
			finally
			{
				il.FlushTrace();
			}
		}

		private static void CreateMapUnpack( Type targetType, SerializerEmitter emitter, CollectionTraits traits )
		{
			CreateMapUnpackFrom( targetType, emitter, traits );
			CreateMapUnpackTo( targetType, emitter, traits );
		}

		private static void CreateMapUnpackFrom( Type targetType, SerializerEmitter emitter, CollectionTraits traits )
		{
			var il = emitter.GetUnpackFromMethodILGenerator();
			try
			{
				/*
				 *	if (!unpacker.IsMapHeader)
				 *	{
				 *		throw SerializationExceptions.NewIsNotMapHeader();
				 *	}
				 *	
				 *	TDictionary<TKey, TValue> dictionary = new ...;
				 *	this.UnpackToCore(unpacker, dictionary);
				 *	return dictionary;
				 */

				if ( targetType.IsInterface || targetType.IsAbstract )
				{
					il.EmitTypeOf( targetType );
					il.EmitAnyCall( SerializationExceptions.NewNotSupportedBecauseCannotInstanciateAbstractTypeMethod );
					il.EmitThrow();
					return;
				}

				il.EmitAnyLdarg( 1 );
				il.EmitGetProperty( Metadata._Unpacker.IsMapHeader );
				var endIf = il.DefineLabel( "END_IF" );
				il.EmitBrtrue_S( endIf );
				il.EmitAnyCall( SerializationExceptions.NewIsNotMapHeaderMethod );
				il.EmitThrow();
				il.MarkLabel( endIf );

				var collection = il.DeclareLocal( targetType, "collection" );
				Emittion.EmitConstruction(
					il,
					collection,
					il0 => Emittion.EmitGetUnpackerItemsCountAsInt32( il0, 1 )
				);

				EmitInvokeMapUnpackToHelper( targetType, emitter, traits, il, 1, il0 => il0.EmitAnyLdloc( collection ) );

				il.EmitAnyLdloc( collection );
				il.EmitRet();
			}
			finally
			{
				il.FlushTrace();
			}
		}

		private static void CreateMapUnpackTo( Type targetType, SerializerEmitter emitter, CollectionTraits traits )
		{
			var il = emitter.GetUnpackToMethodILGenerator();
			try
			{
				EmitInvokeMapUnpackToHelper( targetType, emitter, traits, il, 1, il0 => il0.EmitAnyLdarg( 2 ) );

				il.EmitRet();
			}
			finally
			{
				il.FlushTrace();
			}
		}


		private static void EmitInvokeMapUnpackToHelper( Type targetType, SerializerEmitter emitter, CollectionTraits traits, TracingILGenerator il, int unpackerArgumentIndex, Action<TracingILGenerator> loadCollectionEmitting )
		{
			il.EmitAnyLdarg( unpackerArgumentIndex );
			if ( traits.ElementType.IsGenericType )
			{
				var keyType = traits.ElementType.GetGenericArguments()[ 0 ];
				var valueType = traits.ElementType.GetGenericArguments()[ 1 ];
				var keySerializerGetting = emitter.RegisterSerializer( keyType );
				var valueSerializerGetting = emitter.RegisterSerializer( valueType );
				keySerializerGetting( il, 0 );
				valueSerializerGetting( il, 0 );
				loadCollectionEmitting( il );

				if ( targetType.IsValueType )
				{
					il.EmitBox( targetType );
				}

				il.EmitAnyCall( Metadata._UnpackHelpers.UnpackMapTo_2.MakeGenericMethod( keyType, valueType ) );
			}
			else
			{
				loadCollectionEmitting( il );

				if ( targetType.IsValueType )
				{
					il.EmitBox( targetType );
				}

				il.EmitAnyCall( Metadata._UnpackHelpers.UnpackNonGenericMapTo );
			}
		}

		#endregion -- Maps --

		#region -- Tuples --

		public static SerializerEmitter CreateTupleSerializerCore( Type tupleType, EmitterFlavor emitterFlavor )
		{
			Contract.Requires( tupleType != null );

			var emitter = SerializationMethodGeneratorManager.Get().CreateEmitter( tupleType, emitterFlavor );
			var itemTypes = GetTupleItemTypes( tupleType );
			CreateTuplePack(
				emitter,
				tupleType,
				itemTypes,
				( il, collection ) =>
				{
					il.EmitAnyLdarg( 2 );
					il.EmitAnyStloc( collection );
				}
			);
			CreateTupleUnpack(
				emitter,
				itemTypes
			);

			return emitter;
		}

		private static IList<Type> GetTupleItemTypes( Type tupleType )
		{
			Contract.Assert( tupleType.Name.StartsWith( "Tuple`" ) && tupleType.Assembly == typeof( Tuple ).Assembly );
			var arguments = tupleType.GetGenericArguments();
			List<Type> itemTypes = new List<Type>( tupleType.GetGenericArguments().Length );
			GetTupleItemTypes( arguments, itemTypes );
			return itemTypes;
		}

		private static void GetTupleItemTypes( IList<Type> itemTypes, IList<Type> result )
		{
			int count = itemTypes.Count == 8 ? 7 : itemTypes.Count;
			for ( int i = 0; i < count; i++ )
			{
				result.Add( itemTypes[ i ] );
			}

			if ( itemTypes.Count == 8 )
			{
				var trest = itemTypes[ 7 ];
				Contract.Assert( trest.Name.StartsWith( "Tuple`" ) && trest.Assembly == typeof( Tuple ).Assembly );
				GetTupleItemTypes( trest.GetGenericArguments(), result );
			}
		}

		private static void CreateTuplePack( SerializerEmitter emiter, Type tupleType, IList<Type> itemTypes, Action<TracingILGenerator, LocalBuilder> loadTupleEmitter )
		{
			var il = emiter.GetPackToMethodILGenerator();
			try
			{
				/*
				 * packer.PackArrayHeader( cardinarity );
				 * _serializer0.PackTo( packer, tuple.Item1 );
				 *	:
				 * _serializer6.PackTo( packer, tuple.item7 );
				 * _serializer7.PackTo( packer, tuple.Rest.Item1 );
				 */

				var tuple = il.DeclareLocal( tupleType, "tuple" );
				loadTupleEmitter( il, tuple );
				il.EmitAnyLdarg( 1 );
				il.EmitAnyLdc_I4( itemTypes.Count );
				il.EmitAnyCall( Metadata._Packer.PackArrayHeader );
				il.EmitPop();

				var tupleTypeList = TupleItems.CreateTupleTypeList( itemTypes );

				int depth = -1;
				for ( int i = 0; i < itemTypes.Count; i++ )
				{
					if ( i % 7 == 0 )
					{
						depth++;
					}

					Emittion.EmitSerializeValue(
						emiter,
						il,
						1,
						itemTypes[ i ],
						il0 =>
						{
							il0.EmitAnyLdloc( tuple );

							for ( int j = 0; j < depth; j++ )
							{
								// .TRest.TRest ...
								var rest = tupleTypeList[ j ].GetProperty( "Rest" );
								il0.EmitGetProperty( rest );
							}

							var itemn = tupleTypeList[ depth ].GetProperty( "Item" + ( ( i % 7 ) + 1 ) );
#if DEBUG
							Contract.Assert( itemn != null, tupleTypeList[ depth ].GetFullName() + "::Item" + ( ( i % 7 ) + 1 ) + " [ " + depth + " ] @ " + i );
#endif
							il0.EmitGetProperty( itemn );
						}
					);
				}
				il.EmitRet();
			}
			finally
			{
				il.FlushTrace();
			}
		}

		private static void CreateTupleUnpack( SerializerEmitter emitter, IList<Type> itemTypes )
		{
			CreateTupleUnpackFrom( emitter, itemTypes );
		}

		private static void CreateTupleUnpackFrom( SerializerEmitter emitter, IList<Type> itemTypes )
		{
			var il = emitter.GetUnpackFromMethodILGenerator();
			try
			{
				/*
				 * 	checked
				 * 	{
				 * 		if (!unpacker.IsArrayHeader)
				 * 		{
				 * 			throw SerializationExceptions.NewIsNotArrayHeader();
				 * 		}
				 * 		
				 * 		if ((int)unpacker.ItemsCount != n)
				 * 		{
				 * 			throw SerializationExceptions.NewTupleCardinarityIsNotMatch(n, (int)unpacker.ItemsCount);
				 * 		}
				 * 		
				 *		if (!unpacker.Read())
				 *		{
				 *			throw SerializationExceptions.NewMissingItem(0);
				 *		}
				 *		
				 *		T1 item1;
				 *		if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
				 *		{
				 *			item1 = this._serializer0.UnpackFrom(unpacker);
				 *		}
				 *		else
				 *		{
				 *			using (Unpacker subtreeUnpacker = unpacker.ReadSubtree())
				 *			{
				 *				item1 = this._serializer0.UnpackFrom(subtreeUnpacker);
				 *			}
				 *		}
				 *		
				 *		if (!unpacker.Read())
				 *			:
				 *		
				 *		return new Tuple<...>( item1, ... , new Tuple<...>(...)...);
				 *	}
				 */

				il.EmitAnyLdarg( 1 );
				il.EmitGetProperty( Metadata._Unpacker.IsArrayHeader );
				var endIf = il.DefineLabel( "END_IF" );
				il.EmitBrtrue_S( endIf );
				il.EmitAnyCall( SerializationExceptions.NewIsNotArrayHeaderMethod );
				il.EmitThrow();
				il.MarkLabel( endIf );

				var itemsCount = il.DeclareLocal( typeof( int ), "itemsCount" );
				Emittion.EmitGetUnpackerItemsCountAsInt32( il, 1 );
				il.EmitAnyLdc_I4( itemTypes.Count );
				il.EmitAnyStloc( itemsCount );
				il.EmitAnyLdloc( itemsCount );
				var endIf1 = il.DefineLabel( "END_IF1" );
				il.EmitBeq_S( endIf1 );
				il.EmitAnyLdc_I4( itemTypes.Count );
				il.EmitAnyLdloc( itemsCount );
				il.EmitAnyCall( SerializationExceptions.NewTupleCardinarityIsNotMatchMethod );
				il.EmitThrow();
				il.MarkLabel( endIf1 );

				LocalBuilder[] itemLocals = new LocalBuilder[ itemTypes.Count ];
				int i = 0;
				Action<TracingILGenerator, int> unpackerReading =
					( il1, unpacker ) =>
					{
						il1.EmitAnyLdarg( unpacker );
						il1.EmitAnyCall( Metadata._Unpacker.Read );
						var endIf0 = il1.DefineLabel( "END_IF0" );
						il1.EmitBrtrue_S( endIf0 );
						il1.EmitAnyLdc_I4( i );
						il1.EmitAnyCall( SerializationExceptions.NewMissingItemMethod );
						il1.EmitThrow();
						il1.MarkLabel( endIf0 );
					};

				for ( ; i < itemTypes.Count; i++ )
				{
					itemLocals[ i ] = il.DeclareLocal( itemTypes[ i ], "item" + i );
					Emittion.EmitDeserializeValue( emitter, il, 1, itemLocals[ i ], unpackerReading );
				}

				foreach ( var item in itemLocals )
				{
					il.EmitAnyLdloc( item );
				}

				var tupleTypeList = TupleItems.CreateTupleTypeList( itemTypes );

				for ( int depth = tupleTypeList.Count - 1; 0 <= depth; depth-- )
				{
					il.EmitNewobj( tupleTypeList[ depth ].GetConstructors().Single() );
				}

				il.EmitRet();
			}
			finally
			{
				il.FlushTrace();
			}
		}

		#endregion -- Tuples --

		#region -- Miscs --

		public static bool IsReadOnlyAppendableCollectionMember( MemberInfo memberInfo )
		{
			Contract.Requires( memberInfo != null );

			if ( memberInfo.CanSetValue() )
			{
				// Not read only
				return false;
			}

			Type memberValueType = memberInfo.GetMemberValueType();
			if ( memberValueType.IsArray )
			{
				// Not appendable
				return false;
			}

			CollectionTraits traits = memberValueType.GetCollectionTraits();
			return traits.CollectionType != CollectionKind.NotCollection && traits.AddMethod != null;
		}

		private static void EmitLoadTarget( Type targetType, TracingILGenerator il, int parameterIndex )
		{
			if ( targetType.IsValueType )
			{
				il.EmitAnyLdarga( parameterIndex );
			}
			else
			{
				il.EmitAnyLdarg( parameterIndex );
			}
		}

		private static void EmitLoadTarget( Type targetType, TracingILGenerator il, LocalBuilder local )
		{
			if ( targetType.IsValueType )
			{
				il.EmitAnyLdloca( local );
			}
			else
			{
				il.EmitAnyLdloc( local );
			}
		}

		#endregion -- Miscs --
	}
}
