﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MsgPack.Serialization.GeneratedSerializers {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("MsgPack.Serialization.CodeDomSerializers.CodeDomSerializerBuilder", "0.7.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public class MsgPack_Serialization_PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructorSerializer : MsgPack.Serialization.MessagePackSerializer<MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor> {
        
        private MsgPack.Serialization.MessagePackSerializer<System.Tuple<MsgPack.Serialization.FileSystemEntry, string, string, string, string, string, string>> _serializer0;
        
        private System.Action<MsgPack.Packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor> this_PackValueOfTuple7FirstPolymorphicPackValueOfTuple7FirstPolymorphicDelegate;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>> _packOperationTable;
        
        private System.Reflection.MethodBase _methodBasePolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor_set_Tuple7FirstPolymorphic0;
        
        private System.Action<MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, System.Tuple<MsgPack.Serialization.FileSystemEntry, string, string, string, string, string, string>> this_SetUnpackedValueOfTuple7FirstPolymorphicSetUnpackedValueOfTuple7FirstPolymorphicDelegate;
        
        private System.Action<MsgPack.Unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, int, int> this_UnpackValueOfTuple7FirstPolymorphicUnpackValueOfTuple7FirstPolymorphicDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, int, int>> _unpackOperationTable;
        
        public MsgPack_Serialization_PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructorSerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            MsgPack.Serialization.PolymorphismSchema[] tupleItemsSchema0 = default(MsgPack.Serialization.PolymorphismSchema[]);
            tupleItemsSchema0 = new MsgPack.Serialization.PolymorphismSchema[7];
            MsgPack.Serialization.PolymorphismSchema tupleItemSchema0 = default(MsgPack.Serialization.PolymorphismSchema);
            tupleItemSchema0 = MsgPack.Serialization.PolymorphismSchema.ForPolymorphicObject(typeof(MsgPack.Serialization.FileSystemEntry));
            tupleItemsSchema0[0] = tupleItemSchema0;
            MsgPack.Serialization.PolymorphismSchema tupleItemSchema1 = default(MsgPack.Serialization.PolymorphismSchema);
            tupleItemSchema1 = null;
            tupleItemsSchema0[1] = tupleItemSchema1;
            MsgPack.Serialization.PolymorphismSchema tupleItemSchema2 = default(MsgPack.Serialization.PolymorphismSchema);
            tupleItemSchema2 = null;
            tupleItemsSchema0[2] = tupleItemSchema2;
            MsgPack.Serialization.PolymorphismSchema tupleItemSchema3 = default(MsgPack.Serialization.PolymorphismSchema);
            tupleItemSchema3 = null;
            tupleItemsSchema0[3] = tupleItemSchema3;
            MsgPack.Serialization.PolymorphismSchema tupleItemSchema4 = default(MsgPack.Serialization.PolymorphismSchema);
            tupleItemSchema4 = null;
            tupleItemsSchema0[4] = tupleItemSchema4;
            MsgPack.Serialization.PolymorphismSchema tupleItemSchema5 = default(MsgPack.Serialization.PolymorphismSchema);
            tupleItemSchema5 = null;
            tupleItemsSchema0[5] = tupleItemSchema5;
            MsgPack.Serialization.PolymorphismSchema tupleItemSchema6 = default(MsgPack.Serialization.PolymorphismSchema);
            tupleItemSchema6 = null;
            tupleItemsSchema0[6] = tupleItemSchema6;
            schema0 = MsgPack.Serialization.PolymorphismSchema.ForPolymorphicTuple(typeof(System.Tuple<MsgPack.Serialization.FileSystemEntry, string, string, string, string, string, string>), tupleItemsSchema0);
            this._serializer0 = context.GetSerializer<System.Tuple<MsgPack.Serialization.FileSystemEntry, string, string, string, string, string, string>>(schema0);
            this._methodBasePolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor_set_Tuple7FirstPolymorphic0 = typeof(MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor).GetMethod("set_Tuple7FirstPolymorphic", (System.Reflection.BindingFlags.Instance 
                            | (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)), null, new System.Type[] {
                        typeof(System.Tuple<MsgPack.Serialization.FileSystemEntry, string, string, string, string, string, string>)}, null);
            System.Action<MsgPack.Packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>[] packOperationList = default(System.Action<MsgPack.Packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>[]);
            packOperationList = new System.Action<MsgPack.Packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>[1];
            packOperationList[0] = new System.Action<MsgPack.Packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>(this.PackValueOfTuple7FirstPolymorphic);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>>(1);
            packOperationTable["Tuple7FirstPolymorphic"] = new System.Action<MsgPack.Packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>(this.PackValueOfTuple7FirstPolymorphic);
            this._packOperationTable = packOperationTable;
            System.Action<MsgPack.Unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, int, int>[1];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, int, int>(this.UnpackValueOfTuple7FirstPolymorphic);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, int, int>>(1);
            unpackOperationTable["Tuple7FirstPolymorphic"] = new System.Action<MsgPack.Unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, int, int>(this.UnpackValueOfTuple7FirstPolymorphic);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "Tuple7FirstPolymorphic"};
            this.this_PackValueOfTuple7FirstPolymorphicPackValueOfTuple7FirstPolymorphicDelegate = new System.Action<MsgPack.Packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>(this.PackValueOfTuple7FirstPolymorphic);
            this.this_SetUnpackedValueOfTuple7FirstPolymorphicSetUnpackedValueOfTuple7FirstPolymorphicDelegate = new System.Action<MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, System.Tuple<MsgPack.Serialization.FileSystemEntry, string, string, string, string, string, string>>(this.SetUnpackedValueOfTuple7FirstPolymorphic);
            this.this_UnpackValueOfTuple7FirstPolymorphicUnpackValueOfTuple7FirstPolymorphicDelegate = new System.Action<MsgPack.Unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor, int, int>(this.UnpackValueOfTuple7FirstPolymorphic);
        }
        
        private void PackValueOfTuple7FirstPolymorphic(MsgPack.Packer packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor objectTree) {
            this._serializer0.PackTo(packer, objectTree.Tuple7FirstPolymorphic);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor objectTree) {
            if ((this.OwnerContext.SerializationMethod == MsgPack.Serialization.SerializationMethod.Array)) {
                MsgPack.Serialization.PackHelpers.PackToArray(packer, objectTree, this._packOperationList);
            }
            else {
                MsgPack.Serialization.PackHelpers.PackToMap(packer, objectTree, this._packOperationTable);
            }
        }
        
        private void SetUnpackedValueOfTuple7FirstPolymorphic(MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor unpackingContext, System.Tuple<MsgPack.Serialization.FileSystemEntry, string, string, string, string, string, string> unpackedValue) {
            this._methodBasePolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor_set_Tuple7FirstPolymorphic0.Invoke(unpackingContext, new object[] {
                        unpackedValue});
        }
        
        private void UnpackValueOfTuple7FirstPolymorphic(MsgPack.Unpacker unpacker, MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(unpacker, unpackingContext, this._serializer0, itemsCount, indexOfItem, typeof(System.Tuple<MsgPack.Serialization.FileSystemEntry, string, string, string, string, string, string>), "Tuple7FirstPolymorphic", MsgPack.Serialization.NilImplication.MemberDefault, null, this.this_SetUnpackedValueOfTuple7FirstPolymorphicSetUnpackedValueOfTuple7FirstPolymorphicDelegate);
        }
        
        protected internal override MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor UnpackFromCore(MsgPack.Unpacker unpacker) {
            MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor result = default(MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor);
            result = new MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<MsgPack.Serialization.PolymorphicMemberTypeRuntimeType_Tuple_Tuple7FirstPolymorphicPrivateSetterPropertyAndConstructor>(), this._unpackOperationTable);
            }
        }
    }
}