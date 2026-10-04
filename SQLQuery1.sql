CREATE TABLE [dbo].[Computers] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [IpAddress]   NVARCHAR (50)  NULL,
    [CpuInfo]     NVARCHAR (100) NULL,
    [RamSize]     NVARCHAR (50)  NULL,
    [HddSize]     NVARCHAR (50)  NULL,
    [OsType]      NVARCHAR (50)  NULL,
    [Fio]         NVARCHAR (100) NULL,
    [Email]       NVARCHAR (100) NULL,
    [Phone]       NVARCHAR (50)  NULL,
    [Department]  NVARCHAR (100) NULL,
    [MacAddress]  NVARCHAR (50)  NULL,
    [NetbiosName] NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);