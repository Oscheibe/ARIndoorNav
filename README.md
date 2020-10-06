# ARIndoorNav - ARCampus Navigation

This application was used as a basis for my bachelor's thesis: "Overcoming the problem of uncertain tracking errors in an AR navigation application“. It was initially developed as part of the ARCampus project of the TH Köln – University of Applied Science which seeks to explore the potential of augmented reality in several different contexts. 

The following picture shows the prototype display techniques that were developed for this thesis. The goal for each different technique was to guide the user towards their goal using different approaches which in turn allows for an analysis on how each approach would perform under worse and worse tracking conditions. (More on this ![here](https://github.com/Oscheibe/ARIndoorNav/blob/master/Documentation/Oliver%20Scheibert%20-%202020%20-%20Overcoming%20the%20problem%20of%20uncertain%20tracking%20errors%20in%20an%20AR%20navigation%20application%20(edited).pdf))

![ARCampus Navigation - Display Techniques](https://i.imgur.com/zxTW1qo.png)

This README provides a documentation of the initial project created for my bachelor's thesis: 

The application has been developed using a model-view-presenter (MVP) software architecture pattern that was implemented within Unity. A virtual model of the main building of the campus Gummersbach has been created which is then used to calculate navigation related data, such as paths or destination locations, as well as generate AR guiding elements that are incorporated with the camera feed. To acquire the initial position and rotation (pose) of the user within the building, a marker-based approach is used that relies on detecting the room name from glass plates that are located besides every door within the building. After the initial localization, users can choose a room within the building and start their navigation towards it. A line will be displayed on the ground that leads to the destination with occasional text prompts to help users navigate safely across floor levels.

## Software Architecture
The structure of the architecture is based on a Model-View-Presenter pattern. It divides the software into three parts; the UI (view), the business logic (model) and presenter components to allow communication between view and model. Two of the decision forces before choosing this pattern were the ease of implementation within Unity and the experience of the developer with the pattern.
![ARCampus Navigation - Model-Presenter Dataflow](https://i.imgur.com/OHxVVGQ.png)
To take advantage of the Unity UI, the scene hierarchy structure is closely modeled after the MVP pattern. This is to help give other developer a clear overview of the capabilities of the application without having to read any code. 

## Maintenance 
This sections is meant to give a quick guide on how to develop a custom solution when using this AR navigation approach. 

### Building a Unity model
![ARCampus Navigation - Unity Model](https://i.imgur.com/KvNq5iS.png) 

The model used to determine the path from the user to their destination is based on a floor plan of the building. The plan was added as a texture to a 3D plane which was scaled as needed. Since in Unity’s coordinate system one unit equals one meter, the floor plan size needed to be manually adjusted and the results tested using ARCore. The walls are GameObjects that are extruded from the lines shown on the plan. Doors and other more complicated structures have been simplified as they are not needed for the purpose of this work; Stairs and elevators are modeled as virtual connections between two floors using ARCore’s mesh linking feature. The area around them was marked using NavMesh to scan for users that enter them and adjust the navigation information in response. After the model is complete, a navigation mesh can be generated. 

### The model structure within Unity
![ARCampus Navigation - NavMesh](https://i.imgur.com/GNwZure.png)

The model of the building consists of 4 floors that each have a 3D-ground-plane with a texture that contains a picture of the floorplan. The walls are cubes with the width, depth, and height adjusted based on the plan. They need a box collider component attached to them for NavMesh to calculate the walkable area on the floor plane. They also have to be assigned a “Walls” UI layer so that the ARCore camera can be set to not render them within the video feed. Because the floor plan is divided into east and west, two sets of planes and walls have been created separately, with a third set of GameObjects containing the stairs and elevators. These are modeled as smaller planes within the floor and have a component attached to them that changes the NavMesh area type to a custom “Stairs” or “Elevator” type. To connect two floors together, NavMeshLinks have been used. They are virtual connections that are inlcuded in the navigation path calculations. To allow a path to go through the floor, the ground plane should not have a box collider attached. The navigation mesh will still be calculated on top of it. The stairs could also be modeled as physical steps, but since they are not used as a basis for the navigation information, their representation was kept as simple as necessary.

### Updating the room information
![ARCampus Navigation - Room list JSON](https://i.imgur.com/Qkncuww.png)

A big challenge is to keep the information about the rooms contained in the database up to date. Because of restructuring, the people that work in the rooms and their associated institutes can change several times per year. Changing this information within the application should be easy and not require a change within the data structure itself to keep the maintenance as low as possible. The current method of storing this data is with a JSON file containing the two datasets: “RoomName” and “Description”. The “RoomName” field contains the number of the room. It is important to keep the naming of this field consisted within the app. The name should be a number, or sometimes two, that contains a period instead of a comma. The “Description” field contains any other information about the room, including; the institute name, names of people working in the room, or other descriptive information such as “Toilet”, or “Exit”. This simple structure was made with the intention of keeping it futureproof, without the need to change any part of the code that interacts with it. The stricter rules of the “RoomName” needed to be made in order to identify the locations of the rooms within the model.

### Updating the marker position
The room plates within the building represent the markers for the marker-based approach used for localizing the user. The pose calculation requires the location of the markers within the model. They are modelled as cubes with their GameObject names as the room name and a transform component attached that represents the pose within the model. The general position of the plates is usually besides the room doors, but the exact location had to be manually confirmed. Positioning the marker on the wrong side of the door could lead to an offset of over one meter, which is very noticeable when navigating through the building towards stairs or corners. The current error tolerance of their position is approximately 20cm. The more accurate the position of these markers within the model, the better the pose tracking accuracy.

### OCR key update
The current OCR method relies on the Google Cloud Vision API, which requires a private API key. A key provided by the university is needed in the future, because the current one has a limited amount of usage per month and limited time of how long it can be used for since the start of development. The key is located in a txt file within the “Resources” folder of the project and accessed during the start of the application.

### UI improvements
The current UI design was made based on the intuition and experience of the developer. Since there has not been a formal approach to this design, it could also not be evaluated. The interaction of the user with the AR elements and their exact design need to be formerly tested and improved on in the future. The current goal was to provide a usable interface that displays necessary information about the destination and tracking accuracy.

### Test environment
The application has been developed and tested with the following tools: 
* Unity version 2018.3.14f1 
* Huawei P20 Pro, EMUI version 9.1.0 (Android version 9) 
* ARCore SDK for Android version 1.15 
* ARCore SDK for Unity version 1.15
