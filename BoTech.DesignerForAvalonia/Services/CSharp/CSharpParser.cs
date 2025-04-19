using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using System.Xml.Linq;
using BoTech.DesignerForAvalonia.Models.Project.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace BoTech.DesignerForAvalonia.Services.CSharp;

public class CSharpParser
{
    public static ExtractedClassInfo? GetClassInfoFromAssembly(Assembly assembly, string fullClassName)
    {
        Type? loadedClass = assembly.GetType(fullClassName);
        if (loadedClass != null)
        {
            ExtractedClassInfo extractedClassInfo = new ExtractedClassInfo();
            extractedClassInfo.ClassName = loadedClass.Name;
            extractedClassInfo.Namespace = loadedClass.Namespace ?? string.Empty;
            
            //extractedClassInfo.Documentation = loadedClass.
        }

        return null;
    }
    /// <summary>
    /// Parses the complete given .cs File and extract all necessary information to fill the Models ExtractedClassInfo, ExtractedPropertyInfo and ExtractedMethodInfo.
    /// </summary>
    /// <param name="filePath">The given .cs File path.</param>
    /// <returns>A ExtractedClassInfo ord null when there was an error => Path or File not found</returns>
    /// <exception cref="ArgumentException">When the File is not available</exception>
    public static ExtractedClassInfo? GetClassesInfoFromFile(string filePath)
    {
        if (filePath.EndsWith(".cs") && File.Exists(filePath))
        {
            List<ExtractedClassInfo> result = new List<ExtractedClassInfo>();
            string mainClassName = Path.GetFileNameWithoutExtension(filePath);
            ExtractedClassInfo mainExtractedClass = new ExtractedClassInfo();
            
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath), new CSharpParseOptions(LanguageVersion.Latest));
            
            SyntaxNode root = syntaxTree.GetRoot();
        

            // Extract all class declarations
            IEnumerable<ClassDeclarationSyntax> classDeclarations = root.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>();
            foreach (ClassDeclarationSyntax classDeclaration in classDeclarations)
            {
                ExtractedClassInfo currentExtractedClass = new ExtractedClassInfo();
                
                // Extracting the Documentation
                DocumentationCommentTriviaSyntax? documentation =
                classDeclaration.GetLeadingTrivia().Select(trivia => trivia.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>().FirstOrDefault();
                if(documentation != null) currentExtractedClass.Documentation = documentation.Content.ToString();
                
                // Extracting the Class Name
                currentExtractedClass.ClassName = classDeclaration.Identifier.ToString();
                
                // Extracting the Namespace
                NamespaceDeclarationSyntax? namespaceDeclaration =
                    classDeclaration.GetLeadingTrivia().Select(trivia => trivia.GetStructure())
                        .OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                if(namespaceDeclaration != null) currentExtractedClass.Namespace = namespaceDeclaration.Name.ToString();
                
                // Extracting all Functions
                AddAllFunctionsToClassInfo(currentExtractedClass, classDeclaration);
                // Extracting all Properties
                AddAllPropertiesToClassInfo(currentExtractedClass, classDeclaration);
                
                // Is the parsed Class the Main Class or a subclass
                if (classDeclaration.Identifier.ToString().Equals(mainClassName))
                {
                    // if it's the main class only update all infos.
                    // Do not update the subclass list because there could be subclasses stored already.
                    mainExtractedClass.Methods = currentExtractedClass.Methods;
                    mainExtractedClass.Properties = currentExtractedClass.Properties;
                    mainExtractedClass.Documentation = currentExtractedClass.Documentation;
                    mainExtractedClass.Namespace = currentExtractedClass.Namespace;
                    mainExtractedClass.ClassName = currentExtractedClass.ClassName;
                }
                else
                {
                    mainExtractedClass.SubClasses.Add(currentExtractedClass);
                }
            }
            return mainExtractedClass;
        }
        else
        {
            throw new ArgumentException("CSharpParser: GetClassInfoFromFile: The file path does not end with a .cs or does not exist.");
        }

        return null;
    }
    /// <summary>
    /// This Method is a helper Method to load all declared Methods from the .cs File 
    /// </summary>
    /// <param name="extractedClassInfo">The Class Model where the new methods will be injected.</param>
    /// <param name="classDeclaration">The model which was generated from the source Code and contains info about the class.</param>
    private static void AddAllFunctionsToClassInfo(ExtractedClassInfo extractedClassInfo, ClassDeclarationSyntax classDeclaration)
    {
        IEnumerable<MethodDeclarationSyntax> declaredMethods = classDeclaration.Members.OfType<MethodDeclarationSyntax>();
        foreach (MethodDeclarationSyntax declaredMethod in declaredMethods)
        {
            ExtractedMethodInfo newExtractedMethod = new ExtractedMethodInfo()
            {
                Name = declaredMethod.Identifier.ToString(),
                ReturnType = declaredMethod.ReturnType.ToString(),
            };
            
            // Adding the Documentation
            
            Dictionary<string, string> parameterDocumentation = new Dictionary<string, string>();
            
            DocumentationCommentTriviaSyntax? documentation =
                declaredMethod.GetLeadingTrivia().Select(trivia => trivia.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>().FirstOrDefault();
            if (documentation != null)
            {
                // first let's remove all linebreaks and '///'
                string fullDocumentation = documentation.ToFullString();
                fullDocumentation = fullDocumentation.Replace("\n", string.Empty);
                fullDocumentation = fullDocumentation.Replace("///", string.Empty);
                try
                {
                    XElement xmlDocument = XElement.Parse(fullDocumentation);
                    newExtractedMethod.Documentation = xmlDocument.Element("summary")?.Value ?? string.Empty;
                    foreach (XElement param in xmlDocument.Elements("param"))
                    {
                        parameterDocumentation.Add(param.Attribute("name")?.Value ?? "???", param.Value);
                    }
                }
                catch (Exception e)
                {
                    newExtractedMethod.Documentation = "Error by parsing the XML based documentation: " 
                                                       + e.Message + " \n\n" +
                                                       "The documentation will be displayed as plain text: \n\n" 
                                                       + fullDocumentation;
                }
            }

            // Adding the Params
            foreach (ParameterSyntax parameter in declaredMethod.ParameterList.Parameters)
            {
                string paramDoc = "";
                try
                {
                    paramDoc = parameterDocumentation[parameter.Identifier.ToString()];
                }
                catch (Exception e)
                {
                    paramDoc = "Error by parsing the XML based documentation: " + e.Message;
                }
                ExtractedParamInfo extractedParamInfo = new ExtractedParamInfo()
                {
                    Name = parameter.Identifier.ToString(),
                    Type = parameter.Type?.ToString() ?? "???",
                    DefaultValue = parameter.Default?.Value.ToString() ?? string.Empty,
                    Documentation = paramDoc,
                };
                newExtractedMethod.Parameters.Add(extractedParamInfo);
            }
            extractedClassInfo.Methods.Add(newExtractedMethod);
        }
    }

    /// <summary>
    /// This Method is a helper Method to load all declared <c>Properties</c> from the .cs File 
    /// </summary>
    /// <param name="extractedClassInfo">The Class Model where the new properties will be injected.</param>
    /// <param name="classDeclaration">The model which was generated from the source Code and contains info about the class.</param>
    private static void AddAllPropertiesToClassInfo(ExtractedClassInfo extractedClassInfo, ClassDeclarationSyntax classDeclaration)
    {
        IEnumerable<PropertyDeclarationSyntax> declaredProperties = classDeclaration.Members.OfType<PropertyDeclarationSyntax>();
        foreach (PropertyDeclarationSyntax property in declaredProperties)
        {
            ExtractedPropertyInfo newExtractedProperty = new ExtractedPropertyInfo();
            newExtractedProperty.Name = property.Identifier.ToString();
            
            // Getting the Documentation
            
            DocumentationCommentTriviaSyntax? documentation =
                property.GetLeadingTrivia().Select(trivia => trivia.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>().FirstOrDefault();
            newExtractedProperty.Documentation = documentation?.Content.ToString() ?? string.Empty;
            
            newExtractedProperty.Type = property.Type?.ToString() ?? "???";
            newExtractedProperty.DefaultValue = property.Initializer?.Value.ToString() ?? string.Empty;
            
            // Setting the AccessModifiers and all flags.
            foreach (SyntaxToken modifier in property.Modifiers)
            {
                if (modifier.IsKind(SyntaxKind.PrivateKeyword)) newExtractedProperty.AccessModifier = Modifier.Private;
                if (modifier.IsKind(SyntaxKind.ProtectedKeyword)) newExtractedProperty.AccessModifier = Modifier.Protected;
                if (modifier.IsKind(SyntaxKind.InternalKeyword)) newExtractedProperty.AccessModifier = Modifier.Internal;
                if (modifier.IsKind(SyntaxKind.PublicKeyword)) newExtractedProperty.AccessModifier = Modifier.Public;
                if (modifier.IsKind(SyntaxKind.StaticKeyword)) newExtractedProperty.IsStatic = true;
                if (modifier.IsKind(SyntaxKind.ConstKeyword)) newExtractedProperty.IsConstant = true;
                if (modifier.IsKind(SyntaxKind.ReadOnlyKeyword)) newExtractedProperty.IsReadOnly = true;
            }
            // Checking for {get; set;} definitions
            if (property.AccessorList != null)
            {
                foreach (AccessorDeclarationSyntax accessor in property.AccessorList.Accessors)
                {
                    if(accessor.IsKind(SyntaxKind.GetAccessorDeclaration)) newExtractedProperty.HasGetter = true;
                    if(accessor.IsKind(SyntaxKind.SetAccessorDeclaration)) newExtractedProperty.HasSetter = true;
                }
            }
            extractedClassInfo.Properties.Add(newExtractedProperty);
        }
    }
}