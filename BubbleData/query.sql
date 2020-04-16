-- lecsections.json
select top (1) 
with ties c.[Subject] as [name], c.Code, [data].RemainingSeats as [value]
from Section [data]
full join Class c on c.ClassId=[data].ClassId
where [data].[Type]='LEC' and [data].RemainingSeats>0 
order by row_number() over (partition by
                            c.[Subject], c.Code
                            order by [data].RemainingSeats desc)
FOR JSON AUTO;


-- subjects.json
select c.[Subject] as [name]
from Section s
full join Class c on c.ClassId=s.ClassId
where s.[Type]='LEC' and s.RemainingSeats>0 
group by c.[Subject]
FOR JSON AUTO;