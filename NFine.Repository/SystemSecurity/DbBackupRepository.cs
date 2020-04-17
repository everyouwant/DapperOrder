﻿





using NFine.Code;
using NFine.Data;
using NFine.Domain.Entity.SystemSecurity;
using NFine.IRepository.SystemSecurity;

namespace NFine.Repository.SystemSecurity
{
    public class DbBackupRepository : RepositoryBase<DbBackupEntity>, IDbBackupRepository
    {
        public void DeleteForm(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var dbBackupEntity = db.FindEntity<DbBackupEntity>(keyValue);
                if (dbBackupEntity != null)
                {
                    FileHelper.DeleteFile(dbBackupEntity.F_FilePath);
                }
                db.Delete<DbBackupEntity>(dbBackupEntity);
                db.Commit();
            }
        }
        public void ExecuteDbBackup(DbBackupEntity dbBackupEntity)
        {
            //new DbHelper.ExecuteScalar(CommandType.Text, string.Format("backup database {0} to disk ='{1}'", dbBackupEntity.F_DbName, dbBackupEntity.F_FilePath));
            //dbBackupEntity.F_FileSize = FileHelper.ToFileSize(FileHelper.GetFileSize(dbBackupEntity.F_FilePath));
            //dbBackupEntity.F_FilePath = "/Resource/DbBackup/" + dbBackupEntity.F_FileName;
            //this.Insert(dbBackupEntity);
        }
    }
}
