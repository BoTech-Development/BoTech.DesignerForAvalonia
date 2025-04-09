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

## Issues
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


## Contribution Policy
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
