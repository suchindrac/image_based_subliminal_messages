# Alternative Subliminal Messages

## Summary:

Subliminal messages are a way to communicate with the subconscious mind. There 
 are various ways of doing the same. Most of them rely on the **escape technique**
 which involves crafting messages in a specific way, such that the conscious mind
 **misses** the messages, and it is received by the subconscious mind
 
Another technique of crafting subliminal messages involves super-imposing an 
 image with 1000s of small un-readable text (the actual messages) with very 
 less transparency level. In this way, it has been seen that the subconscious 
 mind accepts the message, whereas the conscious mind can not read it.
 
This is a small tool to embed 1000s of small un-readable text over an image.

## Compilation:

_NOTE: This requires DotNet (csc.exe) to be accessible by path_

Please execute the following commands:

C:\ImageSublim> compile.bat
 
## Execution:

C:\ImageSublim> imageSublim.exe -d subliminal_message_string -f font_name 
                                -fs font_size -p path_to_image -top text_opacity
								-imgop image_opacity -r resize/no/small/
								custom[axb] -1 initial_opacity_of_form 
								-i [true/false]
  
## Example:

imageSublim.exe -d "I am confident" -p ..\b2.jpg -top 0.2 -imgop 0.8 
 -f Ariel -fs 1 -r custom 200x200 -1 1.0 -i true
  

imageSublim.exe -d "I am confident" -p ..\b2.jpg -top 0.2 -imgop 0.8 
 -f Ariel -fs 1 -r small -1 1.0 -i false
  

Initially, you will see a blank screen (this has to be fixed). 
 Just press 'N' to display the image. Below are some of the things that you can 
 do:

* Shift + Space - Focus on the image form 
* N - Show the image and the message
* I - Increases the opacity of the image and the message
* D - Decreases the opacity of the image and the message
* H - Hide the image
* U - Unhide the image
* S - Save the image
* Q - Quit
