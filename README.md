this tutorial build a shooting XR game

Space Gun: https://skfb.ly/6w97U
ID-9 Seeker Droid: https://skfb.ly/osPwt

laser sound: Medium_laser_shoot https://freesound.org/s/814253/


based on this tutorial https://www.youtube.com/watch?v=pZ5vLcyjois

Create a new 3D URP project
Change the build project to "Meta Quest" or "Android"
open project settings and change to   XR  project settings and enable "OpenXR"
fix all warnings and errors
open Meta Quest Project Config and fix all errors
Install in package manager "All in one Meta XR SDK"
fix all warnings and errors, including plugins upgrades

create a new scene
add plain and add a cube to the plane
run the project locally
make sure you see the cube in the scene in the meta simulator
build and run the project on the device
make sure you see the cube in the scene on the device

add meta building blocks: rig, passthrough
run the project locally make sure you see the passthrough in the scene and the plane and the cube

repeat also in the device.


Add a gun as a child of the player right hand anchor - position it properly, and fix rotation if needed (rotation of the object not its parent)
Create a new script called RayGun.cs and add it to the gun game object.
Create laser line prefab and add it to the gun game object.

Add Effect Mesh block to the gun game object, this will add the MRUK to the project.
in the EffectMesh check the "Colliders" and "Hide Mesh" options
