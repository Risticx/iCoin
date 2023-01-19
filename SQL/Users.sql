-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[Users] (
    [ID] int,
    [Username] nvarchar(MAX),
    [Password] nvarchar(MAX),
    [SubscribedCoins] nvarchar(MAX),
    PRIMARY KEY ([ID])
);

