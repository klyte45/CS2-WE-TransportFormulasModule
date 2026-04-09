# WE Transport Formula Module Bridges models

This folder contains the classes and structs that are used to bridge the features from the WE Transport Formula Module to other custom modules. These classes and structs are used to create a bridge between the WE Transport Formula Module and other custom modules, allowing them to communicate with each other and share data.

## Components

These components are unmanaged structs that have the exact same layout as the components in the WE Transport Formula Module. The field order is important and must be maintained to ensure compatibility between the modules. They are meant to be stable as they now forever, and should not be changed. New fields and features should be added in new components, and not in these existing ones.

## Bridge static classes

These static classes are used to bridge the features from the WE Transport Formula Module to other custom modules. They contain static methods that can be called from other custom modules to access the features of the WE Transport Formula Module. These methods are meant to be stable as they now forever, and should not be changed. New fields and features should be added in new static classes, and not in these existing ones.

## Usage

You need to copy both folders into your custom module project and fix their namespaces to match your custom module's namespace. After, include the Patcher class in your module's patching process, and you will be able to use the features of the WE Transport Formula Module in your custom module. 

The components are also available via Write Everywhere formula, since they are components attached to the entities originally - but their actual name have the prefix `WE_TFM_` when attached to the entities, so you need to use that name in the formula to access them. The full list of components and their fields can be found in the source code of the WE Transport Formula Module, in the `Components` folder.