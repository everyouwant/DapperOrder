





using NFine.Code;
using SQLinq;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NFine.Domain.Entity.SystemManage
{
    [Table("Sys_ItemsDetail")]
    [SQLinqTable("Sys_ItemsDetail")]
    [PrimaryKey("F_Id")]
    public class ItemsDetailEntity : IEntity<ItemsDetailEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        [Key]
        public string F_Id { get; set; }
        public string F_ItemId { get; set; }
        [Translate("Sys_Items", "F_ItemId", "F_FullName", "F_Id")]
        public string F_ItemTypeName  { get; set; }
        public string F_ParentId { get; set; }
        public string F_ItemCode { get; set; }
        public string F_ItemName { get; set; }
        public string F_SimpleSpelling { get; set; }
        public bool? F_IsDefault { get; set; }
        public int? F_Layers { get; set; }
        public int? F_SortCode { get; set; }
        public bool? F_DeleteMark { get; set; }
        public bool? F_EnabledMark { get; set; }
        public string F_Description { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        public string F_DeleteUserId { get; set; }
    }
}
