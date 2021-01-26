# ydct
# non uniode
create function [dbo].[non_unicode]( @strInput NVARCHAR(4000) ) returns varchar(4000) as begin declare @p_OriginalString varchar(4000) set @p_OriginalString = CAST(@strInput as varchar) declare @i int = 1; -- must start from 1, as SubString is 1-based declare @OriginalString varchar(4000) = @p_OriginalString Collate SQL_Latin1_General_CP1253_CI_AI; declare @ModifiedString varchar(4000) = '';

while @i <= Len(@OriginalString) begin if SubString(@OriginalString, @i, 1) like '[a-Z]' begin set @ModifiedString = @ModifiedString + SubString(@OriginalString, @i, 1); end set @i = @i + 1; end

return @ModifiedString

end
