# migration rules
1. command : enable-migrations // only once for the first time
2. small change small migration
3. add-migration InitialModel (migration-name) // to keep the initial state of Db 
4. command to override an existing migration: add-migration InitailModel -force
5. update-database -script // will show the updated script but won't update the database
6. update-database // will update the database
7. in migration file there will be two method Up() and Down(). Up() method is called on upgrade of database and Down() method is 
  called on downgrade.
8. get all migration list: Get-Migrations
9. rollback to a specific migration: Update-Database -TargetMigration:"migration_name"