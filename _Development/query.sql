--select MC_ID from MESDP, Node, MECircuit
--where MS_IP = NO_IP and MC_NO = NO_ID and MS_ID = '!ap!Z`>8A=ZN27[F:;[E' and MC_VCID = '1312110589' 


--select MP_ID
--from MEPeer, Node, MECircuit, MESDP
--where --MC_ID = 'b!cVI.-:FrKCTcS1(?rA' and MP_TO_MC <> MC_ID and 
--MP_MS = MS_ID and MS_IP = NO_IP and MC_NO = NO_ID and MP_VCID = MC_VCID
--and MC_VCID = '2013343101'



--select * from MEPeer, MESDP, Node
--where MP_MS = MS_ID and MS_IP = NO_IP and MP_VCID = '1312110590'

--select * from MECircuit where MC_NO = '300'

--select * from Node where NO_Name = 'ME-D2-CKA'

--select * from MECustomer where MU_NO = '300                 ' order by MU_UID asc



--select * from MEQOS where MQ_NO = '300                 ' order by MQ_Type, MQ_Name

--update PEInterface set PI_TO_MI = null where PI_TO_MI in (select MI_ID from MEInterface where MI_NO = '300')
--delete from MEInterface where MI_NO = '300'
--delete from MEQOS where MQ_NO = '300'
--delete from MEPeer where MP_MC in (select MC_ID from MECircuit where MC_NO = '300')
--update MEPeer set MP_TO_MC = null where MP_TO_MC in (select MC_ID from MECircuit where MC_NO = '300')
--delete from MECircuit where MC_NO = '300'
--delete from MECustomer where MU_NO = '300'
--delete from MESDP where MS_NO = '300'

--select MP_VCID, MP_ID from MEPeer, MESDP, Node where MP_MS = MS_ID and MS_IP = NO_IP and NO_ID = '300'
--order by MP_VCID

select * from MESDP where MS_NO = '300'



select 
top 500 MI_ID, MI_Name, MI_Description, NO_Name
from 
MEInterface with (NOLOCK), Node
where 
MI_Description is not null and MI_Type is not null and MI_NO = NO_ID

select * from MEInterface where MI_NO = '300' and MI_Type is not null order by MI_Name

select * from PEInterface where PI_Name not like '%.%' and PI_TO_MI is not null








select MI_ID, MI_NO, MI_Name from MEInterface where MI_Name like 'Ex%' and MI_Name not like '%.%'
and MI_TO_PI is not null

--select * from MEInterface where MI_NO = '300'

select * from MEInterface where MI_NO = '300' order by MI_ID, MI_Name
select * from MECircuit where MC_ID = 'M5<h8K8g]7>K)ud`3t48'

select * from MEInterface where MI_Name = 'Ex1/2/2.3541'

select MI_ID, MI_Name, LEN(MI_Name) as Len from MEInterface where MI_NO = '300'
and MI_Name not like '%.%'
order by Len desc

select 
MI_ID, MI_Name, MI_Description, NO_Name
from 
MEInterface, Node
where 
MI_Description like '%PE-%' and MI_Name not like '%.%' and MI_NO = NO_ID


select NO_ID, NO_Name, LEN(NO_Name) as NO_LEN, PI_ID, PI_Name, LEN(PI_Name) as PI_LEN from Node, PEInterface where NO_Type = 'P' and NO_ID = PI_NO 
and PI_Name not like '%.%' and
(PI_Name like 'Te%' or PI_Name like 'Gi%' or PI_Name like 'Fa%' or PI_Name like 'Et%')
order by NO_LEN desc, PI_LEN desc

select NO_Name, LEN(NO_Name) as NO_LEN, PI_Name, LEN(PI_Name) as PI_LEN, PI_ID, PI_Description from (



select NO_Name, LEN(NO_Name) as NO_LEN, PI_Name, LEN(PI_Name) as PI_LEN, PI_ID, PI_Description from (
select NO_Name, NO_ID from Node where NO_Type = 'P'
union
select NA_Name, NA_NO from NodeAlias, Node where NA_NO = NO_ID and NO_Type = 'P'
) n, PEInterface
where NO_ID = PI_NO and PI_Description is not null and ltrim(rtrim(PI_Description)) <> '' and PI_Name not like '%.%' and
(PI_Name like 'Te%' or PI_Name like 'Gi%' or PI_Name like 'Fa%' or PI_Name like 'Et%')
order by NO_LEN desc, NO_Name, PI_LEN desc, PI_Name

select NO_Name, LEN(NO_Name) as NO_LEN, MI_Name, LEN(MI_Name) as MI_LEN, MI_ID, MI_Description from (
select NO_Name, NO_ID from Node where NO_Type = 'M'
union
select NA_Name, NA_NO from NodeAlias, Node where NA_NO = NO_ID and NO_Type = 'M'
) n, MEInterface
where NO_ID = MI_NO and MI_Description is not null and ltrim(rtrim(MI_Description)) <> '' and MI_Type is not null
order by NO_LEN desc, NO_Name, MI_LEN desc, MI_Name

select MI_Name, MI_Description from MEInterface where MI_Description like '%gi%' and MI_Type is not null


select NO_Name, NA_Name  from Node, NodeAlias where NA_NO = NO_ID and NO_Type = 'M'
order by NO_Name asc

select MI_Name, MI_Description, MI_TO_PI from MEInterface where MI_NO = '300' and MI_Name not like '%.%'

select PI_ID, PI_Name, PI_Description, PI_TO_MI, MI_Name, MI_TO_PI
from PEInterface
left join MEInterface on PI_TO_MI = MI_ID
where PI_PI = 'eSojh8!`p<S)6q+^mS?R' order by PI_Name


select * from MEInterface, Node where MI_NO = NO_ID and MI_Name like '%.%.%' order by MI_NO, MI_Name

select PI_ID, PI_Name from PEInterface where PI_PI = '(S5p`apZEe]fZ-mT_NY5'


select NO_Name, LEN(NO_Name) as NO_LEN, PI_Name, LEN(PI_Name) as PI_LEN, PI_ID, PI_Description from (
select NO_Name, NO_ID from Node where NO_Type = 'P'
union
select NA_Name, NA_NO from NodeAlias, Node where NA_NO = NO_ID and NO_Type = 'P'
) n, PEInterface
where NO_ID = PI_NO and PI_Description is not null and ltrim(rtrim(PI_Description)) <> '' and PI_Name not like '%.%' and
(PI_Name like 'Te%' or PI_Name like 'Gi%' or PI_Name like 'Fa%' or PI_Name like 'Et%')
order by NO_LEN desc, NO_Name, PI_LEN desc, PI_Name

select * from Node where NO_Name = 'ME-D2-CKA'

select MI_Name, MI_Aggregator, MI_Summary_SubInterfaceCount from MEInterface, Node where MI_NO = NO_ID
and NO_Name = 'ME-D2-CKA'
and MI_MI is null
order by MI_Name



select * from NodeSummary order by NS_NO, NS_Key

select a.NO_ID, a.NO_Name, a.NO_Remark, a.NO_TimeStamp, a.NO_LastConfiguration, CASE WHEN a.span < 0 then 0 else a.span end as span from (
select NO_ID, NO_Name, NO_Remark, NO_LastConfiguration, NO_TimeStamp, DateDiff(hour, NO_LastConfiguration, NO_TimeStamp) as span 
from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is not null and NO_LastConfiguration is not null
) a
order by span asc, a.NO_LastConfiguration asc

select top 100 * from Session order by SS_Created desc