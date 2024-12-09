# Main Code Structure-Policy
## Editor
+ For the Editor we are using the following Folders:
1. /ViewModels/Editor/ 
2. /Views/Editor/
3. /Controller/Editor/

**Description:**
1. All the ViewModels for each editor view are stored in this folder.
    Each ViewModel should only provide functionality which is only referred to the View.
    Functionality that is connected to multiple Views has to be programmed in the PreviewController.
2. All the Views for e.g. the Properties View the PreviewView are stored in this folder.
3. All the Controllers which provide Properties(Information) or functionality for all or for some of the Views are stored here.

  