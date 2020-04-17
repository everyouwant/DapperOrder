﻿using NFine.Code;
using SQLinq;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NFine.Domain.Entity.SystemSecurity
{
    [Table("Sys_DbBackup")]
    [SQLinqTable("Sys_DbBackup")]
    [PrimaryKey("F_Id")]
    public class DbBackupEntity : ICreationAudited, IDeleteAudited, IModificationAudited
    {
        [Key]
        public string F_Id { get; set; }
        public string F_BackupType { get; set; }
        public string F_DbName { get; set; }
        public string F_FileName { get; set; }
        public string F_FileSize { get; set; }
        public string F_FilePath { get; set; }
        public DateTime? F_BackupTime { get; set; }
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