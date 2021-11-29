CREATE TABLE [user]
(
    user_id           int IDENTITY (1,1),
    user_key          uniqueidentifier DEFAULT NEWID()   NOT NULL,
    email             varchar(255)                       NOT NULL,
    [password]          varchar(255)                       NOT NULL,
    [name]              varchar(100)                       NOT NULL,
    is_active         bit              DEFAULT 0         NOT NULL,
    created_at        datetime         DEFAULT GETDATE() NOT NULL,
    updated_at        datetime         DEFAULT null      NULL,
    CONSTRAINT ["pk_user"] PRIMARY KEY NONCLUSTERED (user_id)
)
GO

CREATE TABLE user_profile
(
    user_profile_id int IDENTITY (1,1),
    user_id         int                        NOT NULL,
    profile_id      int                        NOT NULL,
    created_at      datetime DEFAULT GETDATE() NOT NULL,
    updated_at      datetime DEFAULT null      NULL,
    CONSTRAINT ["pk_user_profile"] PRIMARY KEY NONCLUSTERED (user_profile_id)
)
GO

CREATE TABLE [profile]
(
    profile_id  int IDENTITY (1,1),
    [description] varchar(100)               NOT NULL,
    created_at  datetime DEFAULT GETDATE() NOT NULL,
    updated_at  datetime                   NULL,
    CONSTRAINT ["pk_profile"] PRIMARY KEY NONCLUSTERED (profile_id)
)
GO

CREATE TABLE task_schedule (
    task_id int IDENTITY(1,1),
    [description] VARCHAR(100),
    created_at        datetime         DEFAULT GETDATE() NOT NULL,
    updated_at        datetime         DEFAULT null      NULL,
    CONSTRAINT ["pk_task_schedule"] PRIMARY KEY NONCLUSTERED (task_id)
    )
GO

CREATE TABLE schedule
(
    schedule_id  int IDENTITY (1,1),
    user_profile_id int NOT NULL,
    task_id int NOT NULL,
    [description] varchar(100)               NOT NULL,
    start_at datetime NOT NULL,
    end_at datetime NOT NULL,
    created_at  datetime DEFAULT GETDATE() NOT NULL,
    updated_at  datetime                   NULL,
    CONSTRAINT ["pk_schedule"] PRIMARY KEY NONCLUSTERED (schedule_id)
)
GO

ALTER TABLE user_profile
    ADD CONSTRAINT ["fk_user_profile_profile"]
        FOREIGN KEY (profile_id) REFERENCES [profile] (profile_id)
GO

ALTER TABLE user_profile
    ADD CONSTRAINT ["fk_user_profile_user"]
        FOREIGN KEY (user_id) REFERENCES [user] (user_id)
GO

ALTER TABLE schedule
    ADD CONSTRAINT ["fk_schedule_task_schedule"]
        FOREIGN KEY (task_id) REFERENCES task_schedule (task_id)
GO

ALTER TABLE schedule
    ADD CONSTRAINT ["fk_schedule_user_profile"]
        FOREIGN KEY (user_profile_id) REFERENCES [user_profile] (user_profile_id)
GO

INSERT INTO [profile] ([description], created_at) VALUES ('Admin', GETDATE()), ('Interviewer', GETDATE()), ('Candidate', GETDATE())
GO
INSERT INTO [user] (email, [password], name, is_active, created_at)
VALUES('admin@gmail.com','admin123@', 'Admin', 1, GETDATE())
GO

INSERT INTO user_profile (user_id, profile_id, created_at) VALUES (1, 1, GETDATE())
GO
