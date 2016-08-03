-███████╗██████╗-██╗████████╗-██████╗-██████╗-██████╗-██████╗-███████╗███████╗███████╗-
-██╔════╝██╔══██╗██║╚══██╔══╝██╔═══██╗██╔══██╗██╔══██╗██╔══██╗██╔════╝██╔════╝██╔════╝-
-█████╗--██║--██║██║---██║---██║---██║██████╔╝██████╔╝██████╔╝█████╗--█████╗--███████╗-
-██╔══╝--██║--██║██║---██║---██║---██║██╔══██╗██╔═══╝-██╔══██╗██╔══╝--██╔══╝--╚════██║-
-███████╗██████╔╝██║---██║---╚██████╔╝██║--██║██║-----██║--██║███████╗██║-----███████║-
-╚══════╝╚═════╝-╚═╝---╚═╝----╚═════╝-╚═╝--╚═╝╚═╝-----╚═╝--╚═╝╚══════╝╚═╝-----╚══════╝-
---------------------------------------------------------------------------------------
---------------------███████╗██████╗-██╗████████╗-██████╗-██████╗----------------------
---------------------██╔════╝██╔══██╗██║╚══██╔══╝██╔═══██╗██╔══██╗---------------------
---------------------█████╗--██║--██║██║---██║---██║---██║██████╔╝---------------------
---------------------██╔══╝--██║--██║██║---██║---██║---██║██╔══██╗---------------------
---------------------███████╗██████╔╝██║---██║---╚██████╔╝██║--██║---------------------
---------------------╚══════╝╚═════╝-╚═╝---╚═╝----╚═════╝-╚═╝--╚═╝---------------------


Hello! This is Christian, the author of EditorPrefs Editor. I hope you're liking the extension!

If you're looking at this in the Unity inspector and wondering what all the visual
noise is, no worries! I just like using ASCII art for titles, but I included a plaintext
version of all sub-heading just in case! :)

We both know you're not here to read about me, so let's get to the interesting stuff!


  ╦ ╦┬ ┬┌─┐┌┬┐  ┬┌─┐  ╔═╗╔═╗╔═╗╔╦╗╦╔╦╗╔═╗╦═╗┌─┐
  ║║║├─┤├─┤ │   │└─┐  ║╣ ╠═╝║╣  ║║║ ║ ║ ║╠╦╝ ┌┘
  ╚╩╝┴ ┴┴ ┴ ┴   ┴└─┘  ╚═╝╩  ╚═╝═╩╝╩ ╩ ╚═╝╩╚═ o
     WHAT IS EPEDITOR?

EPEditor (a.k.a. EditorPrefs Editor, but that's a mouthful so let's go with the short version)
is a small extension for Unity in which you can add a new or existing EditorPref, and keep
track of it.

While working on my editor extensions, I often need to check EditorPrefs, be it for internal
stuff, make sure that preferences are set or initialized correctly, etc. The lack of an
interface to keep track of them has been very frustrating at times.

Enter EPEditor: with it you can add and see EditorPrefs in a visual way. You can create new
Prefs (which will be added to the Unity ones automatically), you can retrieve existing ones,
and you can also edit and remove them.


  ╦ ╦╔═╗╦ ╦  ┌┬┐┌─┐┌─┐┌─┐  ┬┌┬┐  ┬ ┬┌─┐┬─┐┬┌─┌─┐
  ╠═╣║ ║║║║   │││ │├┤ └─┐  │ │   ││││ │├┬┘├┴┐ ┌┘
  ╩ ╩╚═╝╚╩╝  ─┴┘└─┘└─┘└─┘  ┴ ┴   └┴┘└─┘┴└─┴ ┴ o
     HOW DOES IT WORK?

You can use EPEditor in two ways:

The first is by using the interface - you can either add a new EditorPref or retrieve the value
of one whose key and type you know. When they're part of the "database" you can edit and remove
them individually.

The second way to use it is in conjunction with code - the extension includes an EditorPrefs
wrapper called NewEditorPrefs, which works the same as the Unity one (it relies on their own
functions, as a matter of fact), but is also integrated with the interface of the extension, so
that you can add an EditorPref from code and have it automatically added to the EPEditor interface.

It's a fairly small and simple extension, but seeing is believing and I love gifs, so I suggest
going to http://immortalhydra.com/stuff/editorprefs-editor/ to see everything the
extension has to offer :)


  ┬ ┬┬ ┬┌─┐┬─┐┌─┐  ┌─┐┌─┐┌┐┌  ┬  ┌─┐┬┌┐┌┌┬┐  ┌┬┐┌─┐┬─┐┌─┐
  │││├─┤├┤ ├┬┘├┤   │  ├─┤│││  │  ├┤ ││││ ││  ││││ │├┬┘├┤
  └┴┘┴ ┴└─┘┴└─└─┘  └─┘┴ ┴┘└┘  ┴  └  ┴┘└┘─┴┘  ┴ ┴└─┘┴└─└─┘
            ╦╔╗╔╔═╗╔═╗╦═╗╔╦╗╔═╗╔╦╗╦╔═╗╔╗╔┌─┐
            ║║║║╠╣ ║ ║╠╦╝║║║╠═╣ ║ ║║ ║║║║ ┌┘
            ╩╝╚╝╚  ╚═╝╩╚═╩ ╩╩ ╩ ╩ ╩╚═╝╝╚╝ o
     WHERE CAN I FIND MORE INFORMATION?

You can definitely write me if you want to! You can reach me @hherebus on Twitter, or by writing me
a quick email at support@immortalhydra.com

ASCII titles realized with http://patorjk.com/software/taag/ ("ANSI Shadow" and
"Calvin S" styles).