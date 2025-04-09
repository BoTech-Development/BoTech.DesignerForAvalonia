# Contributing to

![Logo](https://github.com/BoTech-Development/BoTech.DesignerForAvalonia/blob/master/BoTech.DesignerForAvalonia/Assets/BoTech_DesignerForAvalonia_WithText_NoBG.png)

You are welcome to contribute to this project. <br/>
First of all it is necessary to know which ways we provide to contribute to this project.

**In this Article**
1. Issues <br/>
1.1 Creating Issues<br/>
1.1.1 Naming conventions for the title<br/>
1.1.2 Feature-Request<br/>
1.1.3 Bug-Report<br/>
1.2 How Issues should be managed.
2. Contribution Policy <br/>
2.1 Become a Contributor <br/>
2.2 Commit Policy <br/>
2.3 Code Style

## <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24"><path fill="orange" d="M20,8H17.19C16.74,7.2 16.12,6.5 15.37,6L17,4.41L15.59,3L13.42,5.17C12.96,5.06 12.5,5 12,5C11.5,5 11.05,5.06 10.59,5.17L8.41,3L7,4.41L8.62,6C7.87,6.5 7.26,7.21 6.81,8H4V10H6.09C6.03,10.33 6,10.66 6,11V12H4V14H6V15C6,15.34 6.03,15.67 6.09,16H4V18H6.81C8.47,20.87 12.14,21.84 15,20.18C15.91,19.66 16.67,18.9 17.19,18H20V16H17.91C17.97,15.67 18,15.34 18,15V14H20V12H18V11C18,10.66 17.97,10.33 17.91,10H20V8M16,15A4,4 0 0,1 12,19A4,4 0 0,1 8,15V11A4,4 0 0,1 12,7A4,4 0 0,1 16,11V15M14,10V12H10V10H14M10,14H14V16H10V14Z" /></svg> Issues
### 1. Create an issue.
If you want to create an issue you can use the Bug-Report Template or the Feature-Request Template. When you want to report an issue it is necessary that you report any important information about the issue.

#### Naming conventions for the title
Each Title of an **official** must follow the following sheme: `{Short Sentence which decribe the conentent} | {Which part of the code should be modified (Views or Backend Classes)} | {Milestone} | {Unique Version (for E.g. V1.0.28)}`

All **unoffcial** community Issues must not follow this sheme. Unofficail means that the Issue was not created by one of the official contributors or of the owner of this porject. Unofficail contributors should use the issues.

**The title will be created by the contributors or the owner of this repository.**

#### Feature-Request
When you want to submitt a feature report you have to first search for an issue on which your feature-request is or could base on. Please use the [Release Project](https://github.com/users/BoTech-Development/projects/1/views/2) to search for the Issue. Write something about the feature. For example you could wirte:
> "There should be support for bindings in the ViewModel. The user should be able to delete, modify or add them"

Then Think about how we can implement this feature (UI and Backend). For example you could write:

> You could contain the Information about the Bindings in the XML-Nodes and update them if necessary. Furthermore you should create the Bindings to create an realistic preview.

If you have any additional Information about the feature, feel free to add them.

#### Bug-Report
1. Please check the [release project](https://github.com/users/BoTech-Development/projects/1/views/2) to identify duplicate issues if you want to report a new bug. Please check all major and minor versions in the project. For example, you might be working with an outdated version, but the bug has already been fixed in a newer version or is on the [roadmap](https://aka.botech.dev/BoTech.DesignerForAvalonia/Roadmap/Current).
2. Now create an Issue with the Bug-Report template. Please fill out all text fields to the best of your knowledge and belief.
### How Issues should be managed
1. When a new issue is opened, somebody on the team adds it to the release project. This also determines when (in which version) and whether the issue will be addressed.
2. The Issue will be categorized in "Backlog🟠", "In review🟣", "Ready to work on🔵", "In progress🟡", "Done🟢"
+ "Backlog🟠" => Issue was created and will be fixed or implemented in this Minor/Mayor Version.
+ "In review🟣" => This means that someone from the team looks at the issue and makes adjustments if errors are present. In this step, the unique version that each issue receives is also determined.
+ "Ready to work on🔵" => The issue has been fully processed and can be implemented.
+ "In progress🟡" => Sombody of the team currently works on this Issue. See who is assigned to this Issue.
+ "Done🟢" => This only applies if the issue has been fully implemented without any build errors and a commit has been made that adds the new code. **After each commit, all parts of the code must be buildable.**


## <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32" width="32" height="32"><path fill="green" d="M7,3A3,3 0 0,1 10,6C10,7.29 9.19,8.39 8.04,8.81C8.58,13.81 13.08,14.77 15.19,14.96C15.61,13.81 16.71,13 18,13A3,3 0 0,1 21,16A3,3 0 0,1 18,19C16.69,19 15.57,18.16 15.16,17C10.91,16.8 9.44,15.19 8,13.39V15.17C9.17,15.58 10,16.69 10,18A3,3 0 0,1 7,21A3,3 0 0,1 4,18C4,16.69 4.83,15.58 6,15.17V8.83C4.83,8.42 4,7.31 4,6A3,3 0 0,1 7,3M7,5A1,1 0 0,0 6,6A1,1 0 0,0 7,7A1,1 0 0,0 8,6A1,1 0 0,0 7,5M7,17A1,1 0 0,0 6,18A1,1 0 0,0 7,19A1,1 0 0,0 8,18A1,1 0 0,0 7,17M18,15A1,1 0 0,0 17,16A1,1 0 0,0 18,17A1,1 0 0,0 19,16A1,1 0 0,0 18,15Z" /></svg> Contribution Policy
### Become a Contributor
What you need to do at least to become a contributor:
1. Check out what you can do => Work on a Issue or implement a part of the [roadmap](https://aka.botech.dev/BoTech.DesignerForAvalonia/Roadmap/Current). Please let us know what you would like to work on.
2. Fork this repo to create your changes.
3. Make a Pull request => Describe how your Code works and on which Issue the code is based on.
4. We will check to PR out and accepted if the code is ok.
5. After that you probably become a contribute.
### Commit Policy
If you want to make a commit, it's important that you base the commit on an issue. Therefore, the commit message must be formatted as follows: Feature and/or enhancement and/or bug #{Issue Number} is now fully implemented.

**❗You are only allowed to commit which is buildable and runnable!**
By setting this rule, we ensure that the **repository is buildable and executable at all time**. This way, others can use the repository at any time to further develop the code or use the program.
### Code Style
+ All members (Functions, Properties, Constants, Enums) must have a xml documentation (summary blocks), if they are not easy to understand.
+ All Classes must have a xml documentation too.
+ Methods names must begin with an Capital letter.
  + Params of the Method must begin with an small letter.
+ Member names must begin with a capital letter.
  + Except private members which must begin with an underscore (_) and a small letter.
+ Differnt words must be seperated by using capital letters.

This article was last updated on 09.04.2025 by BoTech-Development.<br/>
❗These rules may be changed without notice

Powered by:<br/>
![BoTech-Logo](BoTech.DesignerForAvalonia/Assets/BoTechLogoNew.png)

