# Prototyping Virtual Reality Interaction in Unity

In the lecture, we saw three different approaches to interacting with objects in virtual reality scenes. In this practical, we are going to see how to implement these interactions in Unity using the HTC Vive. We'll start off by learning how to use Vive's inbuilt support for these interactions and then learn how they work by re-implementing them ourselves from scratch!

You should work in groups around the computers that have the VR headsets attached. When deploying your VR experiences, make sure that another team member ‘spots’ the user to make sure they don’t bump or trip into any furniture of fellow students.

To get started, make a copy of this repository in your personal GitHub account using the ```Use This Template``` button at the top right of this page, then clone that repository onto the machine you're working on. Then open up the Unity project in the repo and load the ```Starting Scene``` that's in the root directory. 

> **Note** As with last time, if all group members would like a copy of the work at the end of the practical, you can simply push copies the repo up onto everyone's personal accounts at the end of the session (ask if you want help with this).

## Task 1: Using the SteamVR Interaction System to Grab and Throw Objects

SteamVR's inbuilt interaction system implements some of the interaction techniques we learned about in the lecture out of the box. In the first task, we're going to explore how to use this inbuilt functionality to make a scene interactive.

Picking up and throwing objects with the inbuilt interaction system is easy. All you need to do is add two scripts to the game object that you want to pick up. These are:

1. The ```Interactable.cs``` script, which tags the game object as being something that the user can interact with
2. The ```Throwable.cs``` script that implements the actual picking and throwing interaction

Choose one of the books and see if you can make it both "interactable" and "throwable" by doing the above. Once you've got this working, have a play around with some of the options in each component to see how they allow you to reconfigure your interactions. Both of the scripts can be found in: ```SteamVR/InteractionSystem/Core/Scripts```.

SteamVR also provides a range of other classes that implement different types of interactions. For example, the linear and circular drive scripts allow you to implement drawers and hinges. However, for now let's forget the out of the box elements of the interaction system and really get our hands dirty by learning to make our own custom interaction scripts!

## Task 2: Using the HTC Vive Controllers as Proxy Objects

In the remainder of the practical we're going to build on our understanding of the core VR interactions taught in the lecture by building them ourselves from scratch. In doing so, you'll get a really strong understanding of their inner workings, which will help you use them and implement them across multiple VR platforms in the future. You'll also learn how to make your own custom VR interactions, which is important if you want to do things in your project that aren't supported by the out of the box interaction system. 

> **Warning** Before you get started, remove the interactable and throwable components you added in the last task, so things don't get too confusing. Also – and this is crucial – disable the “Hand Physics” components on the Left and Right Hand objects. These already create proxy objects for you, and what’s the fun in that?

The first interaction technique we learned about in the lecture was Proxy Objects. This technique enables interaction with a VR scene by representing some aspect of the user’s body or a controller/tool that they are holding as an object in the physics engine.

In the first task, you should work in your group to implement this proxy object interaction technique with the HTC Vive controllers. Tip: implementing basic proxy object interaction with the Vive controllers should not require you to write any scripts.

Once you have implemented this interaction, take turns to test it out by trying to complete the following actions and observing which it works better for than others:

-	Knock all of the books off the table
-	Stack the books into a pile
-	Throw a book against the wall of the room
-	Push and pull the lever on the right hand side of the table

## Task 3: Picking Up Objects by Parenting their Transform

The second interaction technique we learned about is better suited for picking up the books than proxy objects. This technique involves making a game object that represents the some part of the user’s body or a controller/tool the transform parent of the object that’s to be manipulated. Doing so means that the object being manipulated (the child) will move in synchrony with the object doing the manipulating (the parent).

In this task you should work in your group to implement this interaction technique with the HTC Vive controllers. Implementing this technique is fairly complex and will require a script. Your implementation should be based on the following steps for picking up an object:

1. Detect when the controller comes within contact with an object
2. Wait until the user pulls the trigger button on the controller
3. Set the object that the user is in contact with as the child of the controller’s transform
4. Set the object’s Rigidbody to Kinematic (so it doesn’t fall away to the floor)
5. Releasing an object should be similar, but with steps 3 and 4 undone when the user releases the trigger.

In order to detect when the trigger is pressed, you can either use the pre-existing GrabPinch action or create your own. As we already learned to make our own actions last week, I'd recommend you just use the inbuilt one for now to save time.

The code for detecting whether trigger is down for the GrabPinch action is as follows:

```c#
// check if the trigger is pressed
SteamVR_Input.GetStateDown("GrabPinch", SteamVR_Input_Sources.Any);
```

In case you've forgotten (its been a while since Media Production for Interactive Environments!) here's the code for making one game the child of another:

```c#
// set the game object that the script is on as the parent of another object
GameObject thingBeingGrabbed = // some code to get that game object...
thingBeingGrabbed.transform.parent = transform;
```

> **Note** When doing your implementation you might wonder: should the script be on the controller or the object(s) being grabbed? The answer to that question is either, and both have advantages and disadvantages. Putting the script on the controller is simpler and easier to do. Putting the script on the objects you want to pick up is better for complex scenes and for code-reusability (which is why SteamVR takes this second approach!). Discuss which approach you'd like to take in your group. I'd advise you to put the script on the controller for now, as it'll make your task much simpler and allow you to concentrate on the main learning outcome of the practical, which is learning out the different interactions work.

Once your implementation is complete, take turns to perform the tests listed at the end of the first task to see how the interaction technique performs. Pay particular attention to what happens to the lever (i.e. constrained object) with this technique.

## Task 4: Throwing Objects

You should notice that the interaction technique, as implemented, doesn’t support throwing objects. This is because the object doesn’t retain its parent’s velocity in the physics engine once it’s been removed as the child of the controller.

To enable the throwing of objects using the technique, you need to record the velocity and angular velocity of the controller at the point where the user releases the trigger and then apply these to the object that you want to throw.  In this task, you should extend your implementation to include this functionality. You may find the code snippet below useful when doing this. It'll be a bit more complicated (only a little) to get this working if you've placed your scripts on the objects your interacting with. Ask me for some help if you can't work it out.

```c#
using Valve.VR.InteractionSystem; // include at top of script with other imports

Hand h = GetComponent<Hand>();
Rigidbody toThrow = thingBeingGrabbed.GetComponent<Rigidbody>();
toThrow.velocity = h.trackedObject.GetVelocity();
toThrow.angularVelocity = h.trackedObject.GetAngularVelocity();
```
Take turns to test out whether this leads to a realistic and usable throwing interaction.

## Task 5: Constrained Manipulations with Spring Joints

The final interaction technique we learned about in the lecture enables interactions with constrained objects like the lever (or drawers, doors etc.). This technique worked by attaching invisible springs between the controller(s) and the object that the user is interacting with. These springs are part of the physics engine, and always try and make themselves as short as possible. The result is an interaction technique where the user can ‘pull’ at a door or leaver or ‘hang’ an object from multiple points.

Unity has the springs needed to implement this interaction built into its physics engine – using the ```SpringJoint``` component. Therefore, you can implement the interaction in Unity by following these steps:

1. Add a ```SpringJoint``` component to each of the Vive Controller objects
2. Write a script that attaches the ‘other end’ of the spring (which is set via the connectedBody parameter) to the object that you want to manipulate (i.e. one you are in collision with) when the user pulls the trigger
3. Release the object by setting the ```connectedBody``` parameter to null, when the user releases the trigger

The following code snippet shows how to attach the end of a spring to an object by setting the ```connectedBody``` parameter:

```c#
// attach other end of a spring component to an object
SpringJoint joint = GetComponent<SpringJoint>();
joint.connectedBody = thingBeingGrabbed.GetComponent<Rigidbody>();
```
> **Note** The ```Spring``` parameter of the spring joint sets the tension of the spring. You may need to adjust this this to get responsive interaction. Unity will automatically set the position of the spring on the object you are grabbing for you – so you don’t need to worry about doing this.

Once your implementation is complete, take turns to perform the tests listed at the end of the first task to see how the interaction technique performs.

## Optional Extension: A Nice Half-Way House (Building on the Interactable Object)

If you complete all of the above tasks before the end of the practical, or would like to continue to develop your skills in your free study time, then you should consider experimenting with the following task:

In the last four tasks of the practical today, we made our own custom interactions from scratch. You might be wondering though: is it possible to make my own interactions that are compatible with the SteamVR "Interactable" script, thereby giving me all the nice stuff it brings like the hover overlay? The answer is yes and its not that hard.

I personally found that the best way to learn about extending the interaction system like this is to look at the ```InteractableExample.cs``` script that's found in ```SteamVR\InteractionSystem\Samples\Scripts```. 

This script includes a range of event handlers that are called when certain events are detected in the interaction system. To understand this script – and, therefore, how to make your own ones like it – why not complete try to adapt this script so that it plays sounds when the player hovers over different objects in the scene.
