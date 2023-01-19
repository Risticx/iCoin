-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[Coins] (
    [ID] int,
    [Name] nvarchar(MAX),
    [DateAndTime] datetime2(7),
    [Price] decimal(18,2),
    PRIMARY KEY ([ID])
);

