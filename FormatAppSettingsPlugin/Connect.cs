using System;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace FormatAppSettingsPlugin
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
	    /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
            formatter = new AppSettingsFormatter();
		}

	    /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
	    /// <param name='application'>Root object of the host application.</param>
	    /// <param name='connectMode'>Describes how the Add-in is being loaded.</param>
	    /// <param name='addInInst'>Object representing this Add-in.</param>
	    /// <param name="custom"></param>
	    /// <seealso class='IDTExtensibility2' />
	    public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;
			if(connectMode == ext_ConnectMode.ext_cm_UISetup)
			{
				var contextGUIDS = new object[] { };
				var commands = (Commands2)_applicationObject.Commands;
				const string TOOLS_MENU_NAME = "Tools";

				//Place the command on the tools menu.
				//Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
				var menuBarCommandBar = ((CommandBars)_applicationObject.CommandBars)["MenuBar"];

				//Find the Tools command bar on the MenuBar command bar:
				var toolsControl = menuBarCommandBar.Controls[TOOLS_MENU_NAME];
				var toolsPopup = (CommandBarPopup)toolsControl;

				//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
				//  just make sure you also update the QueryStatus/Exec method to include the new command names.
				try
				{
					//Add a command to the Commands collection:
					var command = commands.AddNamedCommand2(_addInInstance, "FormatAppSettingsPlugin", "FormatAppSettingsPlugin", "Executes the command for FormatAppSettingsPlugin", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported+(int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

					//Add a control for the command to the tools menu:
					if((command != null) && (toolsPopup != null))
					{
						command.AddControl(toolsPopup.CommandBar, 1);
                        var bindings = (object[])command.Bindings;
                        if (0 >= bindings.Length)
                        {
                            // there is no preexisting key binding, so add the default
                            bindings = new object[1];
                            bindings[0] = "Global::SHIFT+ALT+F";
                            command.Bindings = bindings;
                        }
					}
				}
				catch(ArgumentException)
				{
					//If we are here, then the exception is probably because a command with that name
					//  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
				}
			}
		}

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param name='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param name='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param name='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param name='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param name='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param name='commandName'>The name of the command to determine state for.</param>
		/// <param name='neededText'>Text that is needed for the command.</param>
		/// <param name='status'>The state of the command in the user interface.</param>
		/// <param name='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
                if (commandName == "FormatAppSettingsPlugin.Connect.FormatAppSettingsPlugin" && _applicationObject.ActiveWindow != null && _applicationObject.ActiveWindow.Caption.Equals(FILE_NAME, StringComparison.InvariantCultureIgnoreCase))
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
                status = vsCommandStatus.vsCommandStatusInvisible;
            }
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param name='commandName'>The name of the command to execute.</param>
		/// <param name='executeOption'>Describes how the command should be run.</param>
		/// <param name='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param name='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param name='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
			    if (commandName == "FormatAppSettingsPlugin.Connect.FormatAppSettingsPlugin" && _applicationObject.ActiveWindow != null && _applicationObject.ActiveWindow.Caption.Equals(FILE_NAME, StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                        var selection = (TextSelection)_applicationObject.ActiveDocument.Selection;
                        selection.SelectAll();
                        var formattedText = formatter.Tidy(selection.Text);
                        selection.Insert(formattedText, 1);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(String.Format("Error: {0}", e.Message));
                    }

                    handled = true;
                    return;
                }
			    MessageBox.Show(string.Format("Please load {0} to run this plugin.",FILE_NAME));
			}
		}
		private DTE2 _applicationObject;
		private AddIn _addInInstance;
	    AppSettingsFormatter formatter;

        const string FILE_NAME = "AppSettings.Config";

	}
}