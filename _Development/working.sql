
select * from Service where SE_SID = '47022220022380194'

select * from PEInterface where PI_SE = '!=?<hbg]Mg,zg9D5w)9z'
select * from MEInterface where MI_SE = '!=?<hbg]Mg,zg9D5w)9z'
select * from MECircuit where MC_SE = '!=?<hbg]Mg,zg9D5w)9z'



select * from MECircuit where MC_ID = '4s)QP!![+-L?28Q]:Dp6'

select * from MEPeer where MP_MC = '4s)QP!![+-L?28Q]:Dp6'

select * from MESDP where MS_ID = '+9nqk1+D$GJ-OJXI7$iR'

select * from Node where NO_IP = '172.31.70.2'

select * from MECircuit where MC_VCID = '1966613629' and MC_NO = '180                 '



select MI_SE, COUNT(MI_ID) from MEInterface where MI_TO_PI is null and MI_SE is not null
group by MI_SE, MI_NO

select * from MEInterface where MI_SE = '`Lgw/CfrEiT14vl1#5qL'

select * from Service where SE_SID = 'INTG20150123083404'
--!!t~3F$+QtKp7DICol<e

select
n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version, 
c.MC_ID, c.MC_VCID, c.MC_Type, c.MC_Status, c.MC_Protocol
from 
Node n, MECircuit c
where c.MC_SE = '|._@:AL>(_?|Hot.FDB;' and c.MC_SE_Check = 1 and c.MC_NO = n.NO_ID 



union all

select
c.MC_ID, c.MC_VCID, n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version,
c.MC_Type, c.MC_Status, c.MC_Protocol,
i.MI_Name, i.MI_Status, i.MI_Protocol
from
Node n, MEInterface i
left join MECircuit c on i.MI_MC = c.MC_ID
where i.MI_SE = '`Lgw/CfrEiT14vl1#5qL' and i.MI_SE_Check = 1 and i.MI_NO = n.NO_ID and i.MI_TO_PI is null




select MI_Name
from MECircuit, Node
where MC_SE = '|._@:AL>(_?|Hot.FDB;' and MI_MC = MC_ID


select
c.MC_SE, n.NO_Name, c.MC_VCID, COUNT(i.MI_Name)
--n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version, 
--c.MC_ID, c.MC_VCID, c.MC_Type, c.MC_Status, c.MC_Protocol
from 
Node n, MECircuit c
left join MEInterface i on i.MI_MC = c.MC_ID
where 
--c.MC_SE = '|._@:AL>(_?|Hot.FDB;' and 
c.MC_SE_Check = 1 and c.MC_NO = n.NO_ID and c.MC_SE is not null
--order by n.NO_Name, c.MC_VCID
group by c.MC_SE, n.NO_Name, c.MC_VCID


--- C
select
n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version, 
c.MC_ID, c.MC_VCID, c.MC_Type, c.MC_Status, c.MC_Protocol
, COUNT(i2.MI_Name) as I2Count, COUNT(i.MI_Name) as ICount
from 
Node n, MECircuit c
left join MEInterface i on i.MI_MC = c.MC_ID
left join MEInterface i2 on i2.MI_SE = c.MC_SE and i2.MI_ID = i.MI_ID
where 
c.MC_SE = '4p5(S0NRGW2*`_4h.a)R' and 
c.MC_SE_Check = 1 and c.MC_NO = n.NO_ID
group by n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version, 
c.MC_ID, c.MC_VCID, c.MC_Type, c.MC_Status, c.MC_Protocol
order by I2Count desc, ICount desc

select * from PEInterface where PI_SE = '!#;gXJG;!Ebs_]8^E(>W'

--- I
select
i.MI_Name, i.MI_Description, i.MI_Status, i.MI_Protocol,
qosa.MQ_Bandwidth as QInput, qosa.MQ_Name as QInputName,
qosb.MQ_Bandwidth as QOutput, qosb.MQ_Name as QOutputName,
i2.MI_ID as MI2_ID, i2.MI_Description as MI2_Description, i2.MI_Status as MI2_Status, i2.MI_Protocol as MI2_Protocol,
n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version, 
c.MC_ID, c.MC_VCID, c.MC_Description, c.MC_MTU, c.MC_Status, c.MC_Protocol, c.MC_Type,
COUNT(ci.MI_ID) as CInterface, COUNT(cp.MP_ID) as CPeer, aw.AW_AG
from 
Node n, Area ar, AreaWitel aw, MEInterface i
left join MECircuit c on c.MC_ID = i.MI_MC
left join MEQOS qosa on i.MI_MQ_Input = qosa.MQ_ID
left join MEQOS qosb on i.MI_MQ_Output = qosb.MQ_ID
left join MEInterface i2 on i.MI_MI = i2.MI_ID
left join MEInterface ci on ci.MI_MC = c.MC_ID
left join MEPeer cp on cp.MP_MC = c.MC_ID
where
i.MI_SE = '4p5(S0NRGW2*`_4h.a)R' and 
i.MI_SE_Check = 1 and i.MI_TO_PI is null and i.MI_NO = n.NO_ID  and
n.NO_AR = ar.AR_ID and ar.AR_AW = aw.AW_ID
group by
i.MI_Name, i.MI_Description, i.MI_Status, i.MI_Protocol,
qosa.MQ_Bandwidth, qosa.MQ_Name,
qosb.MQ_Bandwidth, qosb.MQ_Name,
i2.MI_ID, i2.MI_Description, i2.MI_Status, i2.MI_Protocol,
n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version, 
c.MC_ID, c.MC_VCID, c.MC_Description, c.MC_MTU, c.MC_Status, c.MC_Protocol, c.MC_Type, aw.AW_AG




select * from MECircuit where M



--select top 100
--a.MI_ID, a.MI_Name, a.MI_Description, NO_Name, a.MI_Status, a.MI_Protocol,
--qosa.MQ_Bandwidth as QInput, qosa.MQ_Name as QInputName,
--qosb.MQ_Bandwidth as QOutput, qosb.MQ_Name as QOutputName,
--a.MI_Rate_Input, a.MI_Rate_Output,
--b.MI_Description as MI2_Description, b.MI_Status as MI2_Status, b.MI_Protocol as MI2_Protocol,
--NO_Manufacture, NO_Version, MC_ID, MC_VCID, MC_Description, MC_MTU, MC_Status, MC_Protocol, MC_Type
--from Node, MEInterface a
--left join MEQOS qosa on a.MI_MQ_Input = qosa.MQ_ID
--left join MEQOS qosb on a.MI_MQ_Output = qosb.MQ_ID
--left join MEInterface b on a.MI_MI = b.MI_ID
--left join MECircuit on a.MI_MC = MC_ID
--where a.MI_NO = NO_ID




--select a.MP_TO_MC, a.MP_TO_Check, a.MP_VCID, a.MP_Protocol, a.MP_Type,
--NO_Name, NO_Manufacture, NO_Version, MC_ID, MC_VCID, MC_Description, MC_MTU, MC_Status, MC_Protocol, MC_Type,
--b.MP_VCID, b.MP_Protocol, b.MP_Type
--from MEPeer a
--left join MECircuit on a.MP_TO_MC = MC_ID
--left join Node on MC_NO = NO_ID
--left join MEPeer b on MC_ID = b.MP_MC and b.MP_TO_MC = a.MP_MC
--where a.MP_MC = 'H2f]j6gWJsO>jI.LfZK^' order by a.MP_TO_MC desc, a.MP_TO_Check desc



--select a.MI_Name, a.MI_Description, a.MI_Status, a.MI_Protocol,
--qosa.MQ_Bandwidth as QInput, qosa.MQ_Name as QInputName,
--qosb.MQ_Bandwidth as QOutput, qosb.MQ_Name as QOutputName,
--a.MI_Rate_Input, a.MI_Rate_Output,
--b.MI_Description as MI2_Description, b.MI_Status as MI2_Status, b.MI_Protocol as MI2_Protocol
--from MEInterface a
--left join MEQOS qosa on a.MI_MQ_Input = qosa.MQ_ID
--left join MEQOS qosb on a.MI_MQ_Output = qosb.MQ_ID
--left join MEInterface b on a.MI_MI = b.MI_ID
--where a.MI_MC = 's0_;!`UiUfPdA[ghZOp<'

--select * from MEInterface where MI_MC = 's0_;!`UiUfPdA[ghZOp<'

--select * from MEInterface where MI_NO = '1421                ' and MI_Name = 'Ex2/2/7'

--select * from Node where NO_ID = '1421                '

--select * from (select MI_ID, MI_NO, MI_Name
--from MEInterface 
--where (MI_Name like '%.%' or MI_Name like 'Ag%') and MI_MI is null) a
--order by MI_NO, MI_Name

--select * from MEInterface where MI_MI is not null

--select * from MEInterface where MI_ID = 'UV=,sEgF[4R#3L,Sc?,I'

--select top 100 MP_ID, MP_VCID, NO_ID from MEPeer, MESDP, Node
--where MP_MS = MS_ID and MS_IP = NO_IP and MP_TO_Check is null








--select * from MEPeer where MP_MC = 's0_;!`UiUfPdA[ghZOp<'

--select * from MECircuit where MC_ID = 'H2f]j6gWJsO>jI.LfZK^'

--select * from MECircuit where MC_ID = 'HgNP+-FeUoR_[CePI*6e'

--select * from MEPeer where MP_MC = 'H2f]j6gWJsO>jI.LfZK^'

--select * from MEPeer where MP_MC = 'HgNP+-FeUoR_[CePI*6e'


----s0_;!`UiUfPdA[ghZOp<

--select top 500 a.MI_ID, a.MI_Name, c.PI_ID, c.PI_Name, c.PI_NO
--from
--MEInterface a, MEInterface b, PEInterface c
--where a.MI_MI = b.MI_ID and b.MI_TO_PI = c.PI_ID
--and a.MI_TO_Check is null

--select * from MEInterface where MI_Name like '%.%.%'

--select * from MEInterface, Node where MI_NO = NO_ID and NO_Name = 'ME2-D1-DRIA' and MI_Name = 'Ex3/1/4'

--select top 500 a.MI_ID, b.MI_ID as MI_MI
--from 
--MEInterface a, MEInterface b
--where a.MI_Name like '%.%' and a.MI_MI is null and a.MI_NO = b.MI_NO and 
--b.MI_Name not like '%.%' and a.MI_Name like b.MI_Name + '.%'

--select NO_ID, NO_Name, LEN(NO_Name) as le, 'O'
--from Node 
--where NO_Remark is null and NO_Active = 1 and NO_TimeStamp is not null and NO_Type = 'P' 
--union 



--select NA_NO, NA_Name, LEN(NA_Name) as le, 'A'
--from NodeAlias, Node
-- where NA_NO = NO_ID and NO_Remark is null and NO_TimeStamp is not null and NO_Active = 1 and NO_Type = 'P' 
-- order by le desc


--select *, LEN(Name) as Len from (
--select NO_ID as ID, NO_Type as Type, NO_Name as Name
--from Node where NO_Active = 1
--union 
--select NO_ID as ID, NO_Type as Type, NA_Name as Name
--from NodeAlias, Node where NO_Active = 1 and NA_NO = NO_ID
--) source
--order by Len desc



--select * from MEInterface where MI_TO_PI is not null

--select * from MEInterface where 

--select * from MEInterface where MI_Name = 'Ex6/2/3' and MI_NO = '305                 '
----Z2SFQMYD9iS$7i>mJaQg
--select * from PEInterface where PI_NO = 'P205                ' and PI_Name = 'Gi2/0/0'
----Q7NO$QsK5VUIu3)`sJRY

--select * from MEInterface where MI_MI = 'Z2SFQMYD9iS$7i>mJaQg'

----!!!!

--select * from PEInterface where PI_PI = 'Q7NO$QsK5VUIu3)`sJRY'

--select * from PEInterface a, PEInterface b where a.PI_ID = '7e;+X91TadV4X4dqCb8^' and a.PI_PI = b.PI_ID



--select count(*) from MEInterface where MI_MI is not null





--select * from Node, PEInterface 
--where PI_NO = NO_ID and NO_Name = 'PE-D3-LBG-INET' and PI_Name = 'Gi1/4'
----TRUNK_PE-D7-AB1-INET/G0/1_TO_ME-D7-AMB/G4/1/11_No1_1G
--select MI_ID, MI_Name, MI_Description, MI_TO_PI, MI_TO_Check 
--from Node, MEInterface where MI_NO = NO_ID and NO_Name = 'ME4-A-JWB-SNT' and MI_Name = 'Ex1/2/5'

--update MEInterface set MI_TO_Check = null where MI_TO_Check = 1 and MI_TO_PI is null and MI_MI is null


--select a.MI_ID, a.MI_Name, a.MI_Description, a.MI_Status, a.MI_Protocol,
--qosa.MQ_Bandwidth as QInput, qosa.MQ_Name as QInputName,
--qosb.MQ_Bandwidth as QOutput, qosb.MQ_Name as QOutputName,
--a.MI_Rate_Input, a.MI_Rate_Output,
--b.MI_Description as MI2_Description, b.MI_Status as MI2_Status, b.MI_Protocol as MI2_Protocol
--from MEInterface a
--left join MEQOS qosa on a.MI_MQ_Input = qosa.MQ_ID
--left join MEQOS qosb on a.MI_MQ_Output = qosb.MQ_ID
--left join MEInterface b on a.MI_MI = b.MI_ID
--where a.MI_MC = '$TlA!MsU^BR^TKW>ULeR'

--select * from MEInterface where MI_ID = '*;.7ekaQT^P+PMomY,As'
--select * from MEInterface where MI_ID = 'Op9UaY?2l<JRo^^KJ]i]'

--select * from MEInterface where MI_Aggregator = 10 and MI_NO = '678'



--select top 500 a.MI_Name, a.MI_Aggregator, b.MI_Name, a.MI_NO-- a.MI_ID, b.MI_ID as MI_MI
--from 
--MEInterface a, MEInterface b
--where 
--a.MI_Aggregator is not null and a.MI_MI is null and a.MI_NO = b.MI_NO and
--b.MI_Name = 'Ag' + CONVERT(varchar(2), a.MI_Aggregator)
--order by MI_NO, MI_Aggregator








