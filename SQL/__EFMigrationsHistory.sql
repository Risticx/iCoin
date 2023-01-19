-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[__EFMigrationsHistory] (
    [MigrationId] nvarchar(150),
    [ProductVersion] nvarchar(32),
    PRIMARY KEY ([MigrationId])
);


INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20230112200921_V1','7.0.2');
INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20230112201504_V2','7.0.2');
INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20230112222954_V3','7.0.2'),('20230114180528_V4','7.0.2');