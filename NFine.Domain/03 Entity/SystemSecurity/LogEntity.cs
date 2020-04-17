using NFine.Code;
using SQLinq;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NFine.Domain.Entity.SystemSecurity
{
    [Table("Sys_Log")]
    [SQLinqTable("Sys_Log")]
    [PrimaryKey("F_Id")]
    public class LogEntity : IEntity<LogEntity>, ICreationAudited
    {
        [Key]
        public string F_Id { get; set; }
        public DateTime? F_Date { get; set; }
        public string F_Account { get; set; }
        public string F_NickName { get; set; }
        public string F_Type { get; set; }
        public string F_IPAddress { get; set; }
        public string F_IPAddressName { get; set; }
        public string F_ModuleId { get; set; }
        public string F_ModuleName { get; set; }
        public bool? F_Result { get; set; }
        public string F_Description { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_CreatorUserId { get; set; }
    }
}
