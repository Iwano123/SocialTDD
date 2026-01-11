BEGIN TRANSACTION;
CREATE TABLE [Follows] (
    [Id] uniqueidentifier NOT NULL,
    [FollowerId] uniqueidentifier NOT NULL,
    [FollowingId] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Follows] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Follows_Users_FollowerId] FOREIGN KEY ([FollowerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Follows_Users_FollowingId] FOREIGN KEY ([FollowingId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_Follows_FollowerId_FollowingId] ON [Follows] ([FollowerId], [FollowingId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260109180016_AddFollowEntity', N'9.0.0');

COMMIT;
GO

