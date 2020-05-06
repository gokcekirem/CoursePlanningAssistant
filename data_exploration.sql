select * from Class
Join Instructor on Class.InstructorId = Instructor.InstructorId
where Instructor.InstructorId = 85;

select * from Instructor
where InstructorId = 85;
