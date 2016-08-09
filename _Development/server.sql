select SS_IPAddress, SR_Query, SR_Match, SR_Count, SR_Created
from share.dbo.Session 
left join center.dbo.SearchResult on SR_Address = SS_IPAddress and DATEDIFF(minute, SR_Created, GETDATE()) < 60
where 
SS_ClientsCount > 0
order by SS_IPAddress