# Bubbly Planner: Course Planning Assistant

Course Planning Assistant for Computer Engineering Design Course at Koç University

## Authors
- H.İrem Gökçek
- Barış Özakar
- Doruk Taneli
- Onur İskenderoğlu

## Before 

You can either

- Copy the mdf file in the [Database](Database) folder to your Windows user's root folder, typically C:\Users\Name1

Or

- To uptade the database, after cloning run Update-Database command in Package Manager Console of Visual Studio. It will create a local database.

- The local database is initally empty, you will need to fill it.
  > You have to change the permission field in *CoursePlanningAssistant/Data/CoursePlannerContext.cs* to true 
  > and run *getAllCareers, getAllInstructors, getAllClasses, getAllSections, getAllPrerequisites* functions one by one with true constraint. 
  
## After

#### Main Page

- Select a class and the conflicting ones will become red. Continue selecting until you have selected all the classes you want.
- Click on **Make Timetable** button to create a set of possible timetables with the selected classes. 
- Click on **Refresh** button to refresh the bubbles and the timetable. 

#### Classes Page

- When you are unsure of which classes are available that semester, search for classes based on their names and subjects. 
  > Autofill functionality will help your search.
 
 #### About Page
 
 - Some information about the program and the authors can also be found here.

## Bubbles

The bubble visualisation is made with *HighCharts.JS*, which is an open source JavaScript library. Each bubble represents a subject (e.g. COMP, HUMS) and the classes contained in them are the points with the same color of their subject.

## Scheduling Algorithm

Scheduling algorithm in *CoursePlanningAssistant/Scheduler/Scheduler.cs* is for eliminating the conflicting classes with the selected classes. The scheduling algorithm works by utilizing the **Collides** function which tells if there is a collision with any two sections. The scheduling algorithm works iteratively, each class is selected one after another and the classes that collide with the chosen class are colored in red in the interactive bubble animation.



