USE [master]
GO
/****** Object:  Database [UpamtiMe]    Script Date: 17-Dec-15 19:36:03 ******/
CREATE DATABASE [UpamtiMe]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'UpamtiMe', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\UpamtiMe.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'UpamtiMe_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\UpamtiMe_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [UpamtiMe] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [UpamtiMe].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [UpamtiMe] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [UpamtiMe] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [UpamtiMe] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [UpamtiMe] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [UpamtiMe] SET ARITHABORT OFF 
GO
ALTER DATABASE [UpamtiMe] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [UpamtiMe] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [UpamtiMe] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [UpamtiMe] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [UpamtiMe] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [UpamtiMe] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [UpamtiMe] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [UpamtiMe] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [UpamtiMe] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [UpamtiMe] SET  DISABLE_BROKER 
GO
ALTER DATABASE [UpamtiMe] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [UpamtiMe] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [UpamtiMe] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [UpamtiMe] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [UpamtiMe] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [UpamtiMe] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [UpamtiMe] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [UpamtiMe] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [UpamtiMe] SET  MULTI_USER 
GO
ALTER DATABASE [UpamtiMe] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [UpamtiMe] SET DB_CHAINING OFF 
GO
ALTER DATABASE [UpamtiMe] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [UpamtiMe] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [UpamtiMe] SET DELAYED_DURABILITY = DISABLED 
GO
USE [UpamtiMe]
GO
/****** Object:  Table [dbo].[Achievements]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Achievements](
	[achievementID] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](50) NOT NULL,
	[description] [varchar](400) NULL,
	[image] [image] NULL,
 CONSTRAINT [PK_Achievements] PRIMARY KEY CLUSTERED 
(
	[achievementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Cards]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cards](
	[cardID] [int] IDENTITY(1,1) NOT NULL,
	[question] [nvarchar](200) NULL,
	[answer] [nvarchar](200) NOT NULL,
	[description] [nvarchar](400) NULL,
	[image] [image] NULL,
 CONSTRAINT [PK_Cards] PRIMARY KEY CLUSTERED 
(
	[cardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Categories]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[categoryID] [int] IDENTITY(1,1) NOT NULL,
	[name] [nchar](10) NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[categoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Courses]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Courses](
	[courseID] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[categoryID] [int] NOT NULL,
	[subcategoryID] [int] NULL,
	[participantCount] [int] NOT NULL,
	[numberOfCards] [int] NOT NULL,
	[creatorID] [int] NOT NULL,
 CONSTRAINT [PK_Courses] PRIMARY KEY CLUSTERED 
(
	[courseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CoursesLevels]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CoursesLevels](
	[coursesLevelsID] [int] IDENTITY(1,1) NOT NULL,
	[courseID] [int] NOT NULL,
	[levelID] [int] NOT NULL,
	[number] [int] NOT NULL,
 CONSTRAINT [PK_CoursesLevels] PRIMARY KEY CLUSTERED 
(
	[coursesLevelsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Friendships]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Friendships](
	[friendshipID] [int] IDENTITY(1,1) NOT NULL,
	[user1ID] [int] NOT NULL,
	[user2ID] [int] NOT NULL,
 CONSTRAINT [PK_Friendship] PRIMARY KEY CLUSTERED 
(
	[friendshipID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Levels]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Levels](
	[levelID] [int] IDENTITY(1,1) NOT NULL,
	[type] [int] NOT NULL,
	[name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Levels] PRIMARY KEY CLUSTERED 
(
	[levelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LevelsCards]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LevelsCards](
	[levelsCardsID] [int] IDENTITY(1,1) NOT NULL,
	[levelID] [int] NOT NULL,
	[cardID] [int] NOT NULL,
	[number] [int] NOT NULL,
 CONSTRAINT [PK_LevelsCards] PRIMARY KEY CLUSTERED 
(
	[levelsCardsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Subcategories]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Subcategories](
	[subcategoryID] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[categoryID] [int] NOT NULL,
 CONSTRAINT [PK_Subcategories] PRIMARY KEY CLUSTERED 
(
	[subcategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[userID] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[surname] [varchar](50) NULL,
	[score] [real] NOT NULL,
	[avatar] [image] NULL,
	[lastLogin] [datetime] NOT NULL,
	[dateRegistered] [datetime] NOT NULL,
	[username] [varchar](30) NOT NULL,
	[password] [varchar](30) NOT NULL,
	[email] [varchar](50) NOT NULL,
	[totalCardsLearned] [int] NOT NULL,
	[bio] [varchar](400) NULL,
	[streak] [int] NOT NULL,
	[location] [varchar](50) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[userID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UsersAchievements]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersAchievements](
	[userAchievementID] [int] IDENTITY(1,1) NOT NULL,
	[userID] [int] NOT NULL,
	[achievementID] [int] NOT NULL,
 CONSTRAINT [PK_UsersAchievements] PRIMARY KEY CLUSTERED 
(
	[userAchievementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsersCards]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersCards](
	[usersCardsID] [int] IDENTITY(1,1) NOT NULL,
	[userID] [int] NOT NULL,
	[cardID] [int] NOT NULL,
	[ignore] [bit] NOT NULL,
	[lastSeen] [datetime] NULL,
	[cardCombo] [int] NOT NULL,
	[nextSee] [datetime] NULL,
 CONSTRAINT [PK_UsersCards] PRIMARY KEY CLUSTERED 
(
	[usersCardsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsersCourses]    Script Date: 17-Dec-15 19:36:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersCourses](
	[usersCoursesID] [int] IDENTITY(1,1) NOT NULL,
	[userID] [int] NOT NULL,
	[courseID] [int] NOT NULL,
	[startDate] [datetime] NOT NULL,
	[score] [int] NOT NULL,
	[lastPlayed] [datetime] NULL,
 CONSTRAINT [PK_UsersCourses] PRIMARY KEY CLUSTERED 
(
	[usersCoursesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Courses]  WITH CHECK ADD  CONSTRAINT [FK_Courses_Categories] FOREIGN KEY([categoryID])
REFERENCES [dbo].[Categories] ([categoryID])
GO
ALTER TABLE [dbo].[Courses] CHECK CONSTRAINT [FK_Courses_Categories]
GO
ALTER TABLE [dbo].[Courses]  WITH CHECK ADD  CONSTRAINT [FK_Courses_Subcategories] FOREIGN KEY([subcategoryID])
REFERENCES [dbo].[Subcategories] ([subcategoryID])
GO
ALTER TABLE [dbo].[Courses] CHECK CONSTRAINT [FK_Courses_Subcategories]
GO
ALTER TABLE [dbo].[CoursesLevels]  WITH CHECK ADD  CONSTRAINT [FK_CoursesLevels_Courses] FOREIGN KEY([courseID])
REFERENCES [dbo].[Courses] ([courseID])
GO
ALTER TABLE [dbo].[CoursesLevels] CHECK CONSTRAINT [FK_CoursesLevels_Courses]
GO
ALTER TABLE [dbo].[CoursesLevels]  WITH CHECK ADD  CONSTRAINT [FK_CoursesLevels_Levels] FOREIGN KEY([levelID])
REFERENCES [dbo].[Levels] ([levelID])
GO
ALTER TABLE [dbo].[CoursesLevels] CHECK CONSTRAINT [FK_CoursesLevels_Levels]
GO
ALTER TABLE [dbo].[Friendships]  WITH CHECK ADD  CONSTRAINT [FK_Friendship_Users] FOREIGN KEY([user1ID])
REFERENCES [dbo].[Users] ([userID])
GO
ALTER TABLE [dbo].[Friendships] CHECK CONSTRAINT [FK_Friendship_Users]
GO
ALTER TABLE [dbo].[Friendships]  WITH CHECK ADD  CONSTRAINT [FK_Friendship_Users1] FOREIGN KEY([user2ID])
REFERENCES [dbo].[Users] ([userID])
GO
ALTER TABLE [dbo].[Friendships] CHECK CONSTRAINT [FK_Friendship_Users1]
GO
ALTER TABLE [dbo].[LevelsCards]  WITH CHECK ADD  CONSTRAINT [FK_LevelsCards_Cards] FOREIGN KEY([cardID])
REFERENCES [dbo].[Cards] ([cardID])
GO
ALTER TABLE [dbo].[LevelsCards] CHECK CONSTRAINT [FK_LevelsCards_Cards]
GO
ALTER TABLE [dbo].[LevelsCards]  WITH CHECK ADD  CONSTRAINT [FK_LevelsCards_Levels] FOREIGN KEY([levelID])
REFERENCES [dbo].[Levels] ([levelID])
GO
ALTER TABLE [dbo].[LevelsCards] CHECK CONSTRAINT [FK_LevelsCards_Levels]
GO
ALTER TABLE [dbo].[Subcategories]  WITH CHECK ADD  CONSTRAINT [FK_Subcategories_Categories] FOREIGN KEY([categoryID])
REFERENCES [dbo].[Categories] ([categoryID])
GO
ALTER TABLE [dbo].[Subcategories] CHECK CONSTRAINT [FK_Subcategories_Categories]
GO
ALTER TABLE [dbo].[UsersAchievements]  WITH CHECK ADD  CONSTRAINT [FK_UsersAchievements_Achievements] FOREIGN KEY([achievementID])
REFERENCES [dbo].[Achievements] ([achievementID])
GO
ALTER TABLE [dbo].[UsersAchievements] CHECK CONSTRAINT [FK_UsersAchievements_Achievements]
GO
ALTER TABLE [dbo].[UsersAchievements]  WITH CHECK ADD  CONSTRAINT [FK_UsersAchievements_Users] FOREIGN KEY([userID])
REFERENCES [dbo].[Users] ([userID])
GO
ALTER TABLE [dbo].[UsersAchievements] CHECK CONSTRAINT [FK_UsersAchievements_Users]
GO
ALTER TABLE [dbo].[UsersCards]  WITH CHECK ADD  CONSTRAINT [FK_UsersCards_Cards] FOREIGN KEY([cardID])
REFERENCES [dbo].[Cards] ([cardID])
GO
ALTER TABLE [dbo].[UsersCards] CHECK CONSTRAINT [FK_UsersCards_Cards]
GO
ALTER TABLE [dbo].[UsersCards]  WITH CHECK ADD  CONSTRAINT [FK_UsersCards_Users] FOREIGN KEY([userID])
REFERENCES [dbo].[Users] ([userID])
GO
ALTER TABLE [dbo].[UsersCards] CHECK CONSTRAINT [FK_UsersCards_Users]
GO
ALTER TABLE [dbo].[UsersCourses]  WITH CHECK ADD  CONSTRAINT [FK_UsersCourses_Courses] FOREIGN KEY([courseID])
REFERENCES [dbo].[Courses] ([courseID])
GO
ALTER TABLE [dbo].[UsersCourses] CHECK CONSTRAINT [FK_UsersCourses_Courses]
GO
ALTER TABLE [dbo].[UsersCourses]  WITH CHECK ADD  CONSTRAINT [FK_UsersCourses_Users] FOREIGN KEY([userID])
REFERENCES [dbo].[Users] ([userID])
GO
ALTER TABLE [dbo].[UsersCourses] CHECK CONSTRAINT [FK_UsersCourses_Users]
GO
USE [master]
GO
ALTER DATABASE [UpamtiMe] SET  READ_WRITE 
GO
