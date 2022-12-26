# Attestation Web API project

## The first release of the Web API project
The first release of the Web API project provides the ability to easily perform CRUD operations for Candidate entities stored in Microsoft Azure Table Storage, following the REST design principles. The project is implemented by following a repository-service pattern and asynchronous methods for storage operations.

## Canidate
### The Candidate entity contains the following properties:
```
Candidate
  - Id, 
  - Name, 
  - Surname, 
  - PhoneNumber, 
  - Seniority (Junior, Mid, Senior)
```
### Following is the list of methods used to provide CRUD functionality:

 - **CreateCandidateAsync()** -  _Create a candidate entity in the Azure Table Storage with provided set of values_
 - **GetCandidateByIdAsync()** - _Retrieve a candidate from the Azure Table Storagae by passing an Id value of the candidate_
 - **GetCandidateBySeniorityAsync()** - _Retrieves a list of candidates from the Azure Table Storage with passed Seniority value(enum)_
 - **GetAllCandidatesAsync()** - _Retrieves all candidate entries from the Azure Table Storage_
 - **UpdateCandidateAsync()** - _Updates candidate entry in the Azure Table Storage by passing an Id value as an identifier and the set of values to update_
 - **DeleteCandidateAsync()** - _Deletes the candidate entry in the Azure Table Storage by passing an Id value as an identifier_

#### Additional notes on the method implementation for Candidate:

- Id is generated automatically during the entity creation in a form of a generated GUID.
- All methods contain validation logic to ensure consistent and reliable data storage.
- UpdateCandidateAsync() was implemented to allow an update on individual properties, depending on which values have been entered in a form. If the value of the property has not been entered that property won't be updated. As the Candidate model design allows for the "PhoneNumber" property to be empty, passing a value of "000000000" in the updates statement removes the value of the property for the updated entity.
- Custom responses for certain scenarios have been implemented to accurately describe the result of a transaction.

## Skill
 ### The Skill entity contains the following properties: 
 ```
Skill
  - Id, 
  - Name, 
  - Tasks (List of Task entities)
```
### Following is the list of methods used to provide CRUD functionality:
 - **CreateSkillAsync()** -  _Create a skill entity in the Azure Table Storage with provided set of values_
 - **GetSkillByIdAsync()** - _Retrieve a skill from the Azure Table Storage by passing an Id value of the skill_
 - **GetSkillByNameAsync()** - _Retrieves a list of skills from the Azure Table Storage with passed Name value(enum)_
 - **GetAllSkillsAsync()** - _Retrieves all skill entries from the Azure Table Storage_
 - **UpdateSkillAsync()** - _Updates skill entry in the Azure Table Storage by passing an Id value as an identifier and the set of values to update, including the value of tasks for the list of task within the skill_
 - **DeleteSkillAsync()** - _Deletes the skill entry in the Azure Table Storage by passing an Id or the Name value as an identifier_

#### Additional notes on the method implementation for Skill:
- Id is generated automatically during the entity creation in a form of a generated GUID.
- All methods contain validation logic to ensure consistent and reliable data storage.
- Skill entities are stored in Table Storage as a separate row for each task they include and are later aggregated to one skill object to be returned as a response.
- Skill can be inserted in the Table Storage without any tasks. If a task is later added, the first entry will be updated to include that task.
- UpdateSkillAsync() was implemented to allow an update on Name properly for a specified Id. Tasks within the skill can be updated as well, by passing the set of values that need to be updated for specified task Id. If the task Id is not found the Table Storage new task with the passed id is created. In the case that the default guid value is passed for the task id, a new id will automatically be generated.
- Custom responses for certain scenarios have been implemented to accurately describe the result of a transaction.

 ### The Task entity contains the following properties: 
 ```
Task
  - Id, 
  - Title, 
  - Level (Junior, Mid, Senior),
  - Description,
  - Code
```
_CRUD functionality needs to be implemented in later release_

## Technologies used for the implementation
- .Net Core
- Azure Table Storage

## Tasks for the future releases 
- [ ] Implement CRUD functionality for Task entities
- [ ] Implement authentication logic via JWT
