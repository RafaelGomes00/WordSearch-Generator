# WordSearch-Generator
 This is a tool project that provides an easy way to create word search boards.

## Installation
 By downloading the project and adding it to your project, it will already be ready to use.

## Usage
 1. To start using this tool, first you have to open the "Board creator" window found in "Window -> Word search creator".
 2. With the window open, you can set your preferences on the window inspector.
 3. When all information has been filled up. Just click the "Generate" button and your board will be randomly generated.
 4. After all that, save your board with "Save board" button, and chose it's path.
 5. With the saved, it will create an Scriptable Object on the path choosen, use this Scriptable Object to access your board informations.

## How does it work
 Foreach word given, the word search generator will randomly assign a direction between Horizontal, Vertical or Diagonal, and one random position on X and Y axis,
 After that, it will try to put the word on that position and orientation, in case that it isn't possible, it will loop through the entire board trying to place the word on the desired orientation,
 In case that it still fails, It will choose another orientation and repeat the process, If it all fails, that word will not be placed.
