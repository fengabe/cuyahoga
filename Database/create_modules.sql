/*                                                          */
/* File generated by "DeZign for databases"                 */
/* Create-date    :3/4/2004                                 */
/* Create-time    :10:58:46 PM                              */
/* project-name   :Cuyahoga                                 */
/* project-author :Martijn Boland                           */
/*                                                          */

CREATE TABLE CM_StaticHtml(
StaticHtmlId serial NOT NULL UNIQUE PRIMARY KEY,
SectionId int4 NOT NULL,
CreatedBy int4 NOT NULL,
ModifiedBy int4,
Title varchar(255),
Content text NOT NULL,
InsertTimestamp timestamp DEFAULT current_timestamp NOT NULL,
UpdateTimestamp timestamp DEFAULT current_timestamp NOT NULL,
FOREIGN KEY (SectionId) REFERENCES Cuyahoga_Section (SectionId),
FOREIGN KEY (CreatedBy) REFERENCES Cuyahoga_User (UserId),
FOREIGN KEY (ModifiedBy) REFERENCES Cuyahoga_User (UserId));


CREATE TABLE CM_ArticleCategory(
ArticleCategoryId serial NOT NULL UNIQUE PRIMARY KEY,
Title varchar(100) NOT NULL,
Summary varchar(255),
Syndicate bool NOT NULL,
InsertTimestamp timestamp DEFAULT current_timestamp NOT NULL,
UpdateTimestamp timestamp DEFAULT current_timestamp NOT NULL);


CREATE TABLE CM_Article(
ArticleId serial NOT NULL UNIQUE PRIMARY KEY,
SectionId int4 NOT NULL,
CreatedBy int4 NOT NULL,
ModifiedBy int4,
ArticleCategoryId int4,
Title varchar(100) NOT NULL,
Summary varchar(255),
Content text NOT NULL,
Syndicate bool NOT NULL,
DateOnline timestamp NOT NULL,
DateOffline timestamp NOT NULL,
InsertTimestamp timestamp DEFAULT current_timestamp NOT NULL,
UpdateTimestamp timestamp DEFAULT current_timestamp NOT NULL,
FOREIGN KEY (ArticleCategoryId) REFERENCES CM_ArticleCategory (ArticleCategoryId),
FOREIGN KEY (SectionId) REFERENCES Cuyahoga_Section (SectionId),
FOREIGN KEY (CreatedBy) REFERENCES Cuyahoga_User (UserId),
FOREIGN KEY (ModifiedBy) REFERENCES Cuyahoga_User (UserId));


CREATE TABLE CM_ArticleComment(
CommentId serial NOT NULL UNIQUE PRIMARY KEY,
ArticleId int4 NOT NULL,
UserId int4,
CommentText varchar(2000) NOT NULL,
InsertTimestamp timestamp DEFAULT current_timestamp NOT NULL,
UpdateTimestamp timestamp DEFAULT current_timestamp NOT NULL,
FOREIGN KEY (ArticleId) REFERENCES CM_Article (ArticleId),
FOREIGN KEY (UserId) REFERENCES Cuyahoga_User (UserId));
