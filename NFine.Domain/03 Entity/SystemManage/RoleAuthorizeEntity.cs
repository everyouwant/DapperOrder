





using NFine.Code;
using SQLinq;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NFine.Domain.Entity.SystemManage
{
    [Table("Sys_RoleAuthorize")]
    [SQLinqTable("Sys_RoleAuthorize")]
    [PrimaryKey("F_Id")]
    public class RoleAuthorizeEntity : IEntity<RoleAuthorizeEntity>, ICreationAudited
    {
        [Key]
        public string F_Id { get; set; }
        public int? F_ItemType { get; set; }
        public string F_ItemId { get; set; }
        public int? F_ObjectType { get; set; }
        public string F_ObjectId { get; set; }
        public int? F_SortCode { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_CreatorUserId { get; set; }
    }
}
