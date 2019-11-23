Create DataBase MyPractiseOne
Go

USE MyPractiseOne
GO
------------------------------------------------------------------------------------------------------------------------------------------------

------Blog Project

SELECT * FROM BlogMaster
SELECT * FROM CategoryMaster
DELETE FROM CategoryMaster
DELETE FROM BlogMaster
GO

Create Table  BlogMaster
(
BlogID Numeric(18,0) Identity PRIMARY KEY,
BlogTitle Varchar(100),
BlogDiscription Varchar(200), 
BlogSequence Numeric(10,0),
SubmissionDate DateTime,
CategoryName Varchar(50),
IsShow Bit,
BlogDetailTittle Varchar(100),
BlogImage Image,
BlogDetails Varchar(Max),
)
Go

ALTER TABLE BlogMaster
ALTER COLUMN cat Varchar(Max);
GO

ALTER TABLE BlogMaster
ADD ImageName Varchar(50),ImageExtension NVarchar(200)
GO

ALTER TABLE BlogMaster DROP COLUMN CategoryName;
GO

ALTER TABLE BlogMaster
ADD CatID int
GO

ALTER PROC Sp_BlogMaster_InsertUpdate
(
	@BlogID Numeric(18,0),
	@BlogTitle Varchar(100),
	@BlogDiscription Varchar(200), 
	@BlogSequence Numeric(10,0),
	@SubmissionDate DateTime,
	@IsShow Bit,
	@BlogDetailTittle Varchar(100),
	@BlogImage Image,
	@BlogDetails Varchar(Max),
	@ImageName Varchar(50),
	@ImageExtension NVarchar(200),
	@CatID int
)
AS
	BEGIN
		Declare @Msg Varchar(Max)='',@Seq Numeric(10,0),@Status Int=0,@Focus Varchar(50)='txtCountryCode'
		If isnull(@BlogTitle,'')=''
		Begin
			Set @Msg='Please Enter Blog Title..!'
		End		
		Else IF @BlogID=0
		Begin
		    UPDATE BlogMaster SET BlogSequence=BlogSequence+1 WHERE BlogSequence>=@BlogSequence
			INSERT INTO BlogMaster(BlogTitle,BlogDiscription,BlogSequence,SubmissionDate,IsShow,BlogDetailTittle,BlogImage,BlogDetails,ImageName,ImageExtension,CatID)
			VALUES(@BlogTitle,@BlogDiscription,@BlogSequence,@SubmissionDate,@IsShow,@BlogDetailTittle,@BlogImage,@BlogDetails,@ImageName,@ImageExtension,@CatID)
			set @BlogID= SCOPE_IDENTITY()
			If @BlogID >0
			Begin
				Set @Status=1	
				Set @Msg='Record Inserted Successfully..!'
			End
		End
	Else
	Begin
	    SELECT @Seq=BlogSequence FROM BlogMaster Where BlogID=@BlogID
		UPDATE BlogMaster SET BlogSequence=BlogSequence-1 WHERE BlogSequence>=@Seq
	    UPDATE BlogMaster SET BlogSequence=BlogSequence+1 WHERE BlogSequence>=@BlogSequence
		UPDATE BlogMaster SET BlogTitle=@BlogTitle,BlogDiscription=@BlogDiscription,BlogSequence=@BlogSequence,SubmissionDate=@SubmissionDate,
		IsShow=@IsShow,BlogDetailTittle=@BlogDetailTittle,BlogImage=@BlogImage,BlogDetails=@BlogDetails,
		ImageName=@ImageName,ImageExtension=@ImageExtension,CatID=@CatID
		WHERE BlogID=@BlogID
		Set @Status=2
		Set @Msg='Record Updated Successfully..!'
	End
	Select @Msg As Result,@Status As [Status]
End
GO

CREATE PROC Sp_BlogMaster_Get
AS
	BEGIN
		SELECT BlogMaster.BlogID As [Blog ID],BlogMaster.BlogTitle AS [Blog Title],BlogMaster.BlogDiscription AS [Blog Discription],
		BlogMaster.BlogSequence AS [Blog Sequence],CategoryMaster.CategoryName AS [Category Name], convert(varchar(20),BlogMaster.SubmissionDate,106) 
		As [Submision Date],BlogMaster.IsShow AS [Is Show],BlogMaster.BlogImage AS [Blog Image],BlogMaster.BlogDetails As [Blog Details]   
		From BlogMaster inner join CategoryMaster 
		on BlogMaster.CatID=CategoryMaster.CatID WHERE IsShow ='1'
	
	END
GO

SP_HELPTEXT Sp_BlogMaster_BlogByCatName
GO

ALTER PROC Sp_BlogMaster_BlogByCatName
@CatID NUMERIC(18,0)
AS
	BEGIN
		Select *,convert(varchar(50),SubmissionDate,106) as [NewSubmissionDate] From BlogMaster
		INNER JOIN CategoryMaster ON BlogMaster.CatID=CategoryMaster.CatID
		 Where IsShow=1 and CategoryMaster.CatID=@CatID
	END	

ALTER PROC Sp_CategoryMaster_GetALL
AS
	BEGIN
		SELECT CategoryMaster.*,(SELECT count(CatID) FROM BlogMaster WHERE CategoryMaster.CatID = BlogMaster.CatID) as CategoryPostCount 
	     FROM CategoryMaster
	END
GO

ALTER PROC Sp_BlogMaster_GetALL
AS
	BEGIN
		Select *,convert(varchar(50),SubmissionDate,106) as [NewSubmissionDate] From BlogMaster INNER JOIN CategoryMaster 
		ON BlogMaster.CatID=CategoryMaster.CatID where  IsShow=1
	END
GO

CREATE PROC Sp_BlogMaster_Edit
(
@BlogID Numeric(18,0)
)
AS
	BEGIN
		Select * From BlogMaster Where BlogID=@BlogID
	END
GO

ALTER PROC Sp_BlogMaster_Delete
(
@BlogID Numeric(18,0)
)
AS
	BEGIN
	  Declare @Seq Numeric(10,0)
	   SELECT @Seq=BlogSequence FROM BlogMaster Where BlogID=@BlogID
		UPDATE BlogMaster SET BlogSequence=BlogSequence-1 WHERE BlogSequence>=@Seq
		Delete  From BlogMaster Where BlogID=@BlogID
	END
GO

SP_HELPTEXT Sp_BlogMaster_Get
GO

ALTER PROC Sp_BlogMaster_DetailsGetByID
@BlogID NUMERIC(18,0)
AS
	BEGIN
		Select *,convert(varchar(50),SubmissionDate,106) as [NewSubmissionDate] From BlogMaster Where BlogID=@BlogID
	END
Go

ALTER PROC Sp_BlogMaster_GetALL
AS
	BEGIN
		Select *,convert(varchar(50),SubmissionDate,106) as [NewSubmissionDate] From BlogMaster where  IsShow=1
	END
Go

CREATE PROC Sp_BlogMaster_DownloadDetails
(
@BlogID NUMERIC(18,0)
)
AS
	BEGIN
		Select BlogID,BlogImage,ImageName,ImageExtension From BlogMaster where BlogID=isnull(@BlogID,BlogID) 
	END
Go

Create Table  CategoryMaster
(
CatID Numeric(18,0) Identity PRIMARY KEY,
CatName Varchar(100)
)
Go

ALTER TABLE CategoryMaster
ADD [Sequence] Numeric(18,0)
GO

SP_RENAME 'CategoryMaster.CatName', 'CategoryName', 'COLUMN';
Go

ALTER PROC Sp_CategoryMaster_InsertUpdate
(
	@CatID Numeric(18,0),
	@CategoryName Varchar(100),
	@Sequence Numeric(18,0)
)
AS
	BEGIN
		Declare @Msg Varchar(Max)='',@Seq Numeric(18,0), @Status Int=0,@Focus Varchar(50)='txtCountryCode'
		If isnull(@CategoryName,'')=''
		Begin
			Set @Msg='Please Enter Blog Title..!'
		End		
		Else IF @CatID=0
		Begin

			UPDATE CategoryMaster SET [Sequence]=[Sequence]+1 WHERE [Sequence]>=@Sequence

			INSERT INTO CategoryMaster(CategoryName,[Sequence])
			VALUES(@CategoryName,@Sequence)
			set @CatID= SCOPE_IDENTITY()
			If @CatID >0
			Begin
				Set @Status=1	
				Set @Msg='Record Inserted Successfully..!'
			End
		End
	Else
	Begin
	   
	    SELECT @Seq=[Sequence] FROM CategoryMaster Where CatID=@CatID
		UPDATE CategoryMaster SET [Sequence]=[Sequence]-1 WHERE [Sequence]>=@Seq
	    UPDATE CategoryMaster SET [Sequence]=[Sequence]+1 WHERE [Sequence]>=@Sequence
		UPDATE CategoryMaster SET CategoryName=@CategoryName,[Sequence]=@Sequence
		WHERE CatID=@CatID
		Set @Status=2
		Set @Msg='Record Updated Successfully..!'
	End
	Select @Msg As Result,@Status As [Status]
End
GO

CREATE PROC Sp_CategoryMaster_GetALL
AS
	BEGIN
		Select * From CategoryMaster 
	END
Go

ALTER PROC Sp_CategoryMaster_Delete
(
@CatID Numeric(18,0),
@Sequence Numeric(18,0)
)
AS
	BEGIN
	Declare @Seq Numeric(18,0)
	 SELECT @Seq=[Sequence] FROM CategoryMaster Where CatID=@CatID
	UPDATE CategoryMaster SET [Sequence]=[Sequence]-1 WHERE [Sequence]>=@Seq
	
		Delete  From CategoryMaster Where CatID=@CatID
	END
GO

CREATE PROC Sp_CategoryMaster_Edit
(
@CatID Numeric(18,0)
)
AS
	BEGIN
		Select * From CategoryMaster Where CatID=@CatID
	END
GO



Create Table  Oppinion
(
OppinionID Numeric(18,0) Identity PRIMARY KEY,
Oppinions Varchar(100),
UserName Varchar(50),
Email Varchar(50),
BlogID Numeric(18,0),
FOREIGN KEY (BlogID) REFERENCES BlogMaster(BlogID)
)
Go

Alter Table Oppinion
Add IsVerfiy Bit NOT NULL DEFAULT 0
Go

Alter Table Oppinion
ALTER TABLE Oppinion
ALTER COLUMN Oppinions Varchar(max)
GO


select * from Oppinion
ALTER PROC Sp_Blog_Oppinion_InsertUpdate 
(
	@OppinionID Numeric(18,0),
	@Oppinions Varchar(max),
	@UserName Varchar(50),
	@Email Varchar(50),
	@BlogID Numeric(18,0)
)
AS
	BEGIN
		Declare @Msg Varchar(Max)='',@Status Int=0,@Focus Varchar(50)='txtOpinion'
		
		 IF @OppinionID=0
		Begin
			INSERT INTO Oppinion(Oppinions,UserName,Email,BlogID)
			VALUES(@Oppinions,@UserName,@Email,@BlogID)
			set @OppinionID= SCOPE_IDENTITY()
			If @OppinionID >0
			Begin
				Set @Status=1	
				Set @Msg='Record Inserted Successfully..!'
			End
		End
	Else
	Begin
		UPDATE Oppinion SET Oppinions=@Oppinions,UserName=@UserName,Email=@Email,BlogID=@BlogID
		WHERE OppinionID=@OppinionID
		Set @Status=2
		Set @Msg='Record Updated Successfully..!'
	End
	Select @Msg As Result,@Status As [Status]
End
GO

ALTER PROC Sp_Blog_Oppinion_Get 
@BlogID int
AS
BEGIN
	Declare @Totalresponse Int=0
	select * iNTO #TEMP from Oppinion where BlogID=@BlogID
	select @Totalresponse=cOUNT(*) FROM #TEMP  wHERE  BlogID=@BlogID
	sELECT @Totalresponse as TotalResponse,* FROM #TEMP wHERE  BlogID=@BlogID
END
GO

ALTER PROC Sp_Oppinion_GetALL
AS
BEGIN
	SELECT '<input type=''checkbox'' name=''chkdverify'' value='''+convert(varchar(60),Oppinion.OppinionID)+''' id='''+convert(varchar(60),Oppinion.OppinionID)+''' />' AS [Select], OppinionID AS [Oppinion ID],Oppinions,UserName AS[Name],Email AS [Email ID],BlogID AS [Blog ID] FROM Oppinion
END

CREATE PROC Sp_Oppinio_Delete
(
@OppinionID Numeric(18,0)
)
AS
BEGIN
	Delete  From Oppinion Where OppinionID=@OppinionID
END

CREATE PROC Sp_Oppinio_Edit 1
(
@OppinionID Numeric(18,0)
)
AS
BEGIN
	Select *  From Oppinion Where OppinionID=@OppinionID
END

CREATE PROC Sp_Oppinio_Update
(
@OppinionID Numeric(18,0)
)
AS
BEGIN
	UPDATE Oppinion SET IsVerfiy=1	WHERE OppinionID=@OppinionID
END

Alter PROC Sp_Oppinion_GetALL
AS
BEGIN
	SELECT '<input type=''checkbox'' name=''chkdverify'' value='''+convert(varchar(60),Oppinion.OppinionID)+''' id='''+convert(varchar(60),Oppinion.OppinionID)+''' />' AS [Select], OppinionID AS [Oppinion ID],Oppinions,UserName AS[Name],Email AS [Email ID],
	BlogID AS [Blog ID] FROM Oppinion where IsVerfiy=0
END
Go

CREATE PROC Sp_Oppinion_GetALL_IsVerified
AS
BEGIN
	SELECT '<input type=''checkbox'' name=''chkdverify'' value='''+convert(varchar(60),Oppinion.OppinionID)+''' id='''+convert(varchar(60),Oppinion.OppinionID)+''' />' AS [Select], OppinionID AS [Oppinion ID],Oppinions,UserName AS[Name],Email AS [Email ID],
	BlogID AS [Blog ID] FROM Oppinion where IsVerfiy=1
END
GO
	

sELECT * FROM BlogMaster
select * FROM Oppinion
DELETE FROM CategoryMaster
DELETE FROM 



sp_helptext Sp_BlogMaster_BlogByCatName




ALTER PROC Sp_BlogMaster_BlogByCatName
@CatID NUMERIC(18,0)
AS
	BEGIN
		Select *,convert(varchar(50),SubmissionDate,106) as [NewSubmissionDate],(SELECT count(BlogID) FROM Oppinion WHERE Oppinion.BlogID = BlogMaster.BlogID) as TotalResponse FROM BlogMaster 
		INNER JOIN CategoryMaster ON BlogMaster.CatID=CategoryMaster.CatID
		 Where IsShow=1 and CategoryMaster.CatID=@CatID
	END

	CREATE PROC Sp_BlogMaster_GetALL
AS
	BEGIN
	
		SELECT BlogMaster.*,CategoryMaster.*,convert(varchar(50),SubmissionDate,106) as [NewSubmissionDate],(SELECT count(BlogID) FROM Oppinion WHERE Oppinion.BlogID = BlogMaster.BlogID) as TotalResponse FROM BlogMaster 
		INNER JOIN CategoryMaster 
		ON BlogMaster.CatID=CategoryMaster.CatID
	     	 where  IsShow=1
		 
	END
	alter PROC Sp_Blog_Oppinion_Get 78
@BlogID int
AS
BEGIN
	Declare @Totalresponse Int=0
	select * iNTO #TEMP from Oppinion where BlogID=@BlogID and IsVerfiy=1
	select @Totalresponse=cOUNT(*) FROM #TEMP  wHERE  BlogID=@BlogID
	sELECT @Totalresponse as TotalResponse,* FROM #TEMP wHERE  BlogID=@BlogID
END



























Alter PROC Sp_BlogMaster_GetALL
AS
	BEGIN
	
		SELECT BlogMaster.*,CategoryMaster.*,convert(varchar(50),SubmissionDate,106) as [NewSubmissionDate],(SELECT count(BlogID) FROM Oppinion WHERE Oppinion.BlogID = BlogMaster.BlogID) as TotalResponse FROM BlogMaster 
		INNER JOIN CategoryMaster 
		ON BlogMaster.CatID=CategoryMaster.CatID
	     	 where  IsShow=1
		 
	END

	Digital Marketing Trends That Will Own 2019


	SELECT * FROM BlogMaster

		CREATE PROC Sp_BlogMaster_BlogByCatName
@CatID NUMERIC(18,0)
AS
	BEGIN
		Select *,convert(varchar(50),SubmissionDate,106) as [NewSubmissionDate],(SELECT count(BlogID) FROM Oppinion WHERE Oppinion.BlogID = BlogMaster.BlogID) as TotalResponse FROM BlogMaster 
		INNER JOIN CategoryMaster ON BlogMaster.CatID=CategoryMaster.CatID
		 Where IsShow=1 and CategoryMaster.CatID=@CatID
	END

alter PROCEDURE Sp_BlogMaster_BlogBySearchFilter 
@BlogTitle varchar(100)
as  
BEGIN  
Select *,convert(varchar(50),SubmissionDate,106) as [NewSubmissionDate],(SELECT count(BlogID) FROM Oppinion WHERE Oppinion.BlogID = BlogMaster.BlogID) as TotalResponse FROM BlogMaster 
		INNER JOIN CategoryMaster ON BlogMaster.CatID=CategoryMaster.CatID
		 Where IsShow=1 and BlogTitle Like '%'+ @BlogTitle +'%'
END  
