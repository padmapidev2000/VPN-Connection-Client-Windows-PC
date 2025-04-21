# VPN Client Application for Windows

A Windows application that allows users to create and manage VPN connections (PPTP or L2TP) with specific configuration settings.

## Features

1. **Create VPN Connection**
   - VPN name: User input or fixed value (default: MyVPN)
   - Server address: User input or loaded from config file
   - Connection method: PPTP or L2TP selectable
   - Username/password can be saved

2. **Set VPN Properties Automatically**
   - VPN type: "Layer 2 Tunneling Protocol with IPSec (L2TP/IPSec)"
   - Data encryption: Enable optional encryption (connect without encryption)
   - Authentication protocol settings:
     - Check "Allow the following protocols"
     - Enable Microsoft CHAP Version 2 (MS-CHAP v2)
     - Uncheck CHAP / PAP
     - Disable automatic login

3. **VPN Connection Button**
   - Attempt to connect to VPN when clicking the button
   - Display connection success/failure message

4. **Delete VPN Button**
   - Delete registered VPN connection

## Requirements

- Windows 10 or later
- .NET 6.0 or later

## Building the Application

### For Windows Development

1. Open the solution in Visual Studio 2022 or later
2. Modify the Program.cs file to uncomment the Windows Forms code:
   ```csharp
   // Uncomment these lines:
   using System.Windows.Forms;
   ApplicationConfiguration.Initialize();
   Application.Run(new MainForm());
   ```
3. Change the project file (VpnClientApp.csproj) to use Windows Forms:
   ```xml
   <PropertyGroup>
     <OutputType>WinExe</OutputType>
     <TargetFramework>net6.0-windows</TargetFramework>
     <ImplicitUsings>enable</ImplicitUsings>
     <Nullable>enable</Nullable>
     <UseWindowsForms>true</UseWindowsForms>
   </PropertyGroup>
   ```
4. Build the solution
5. Run the application

## Usage

1. Launch the application
2. Enter the VPN name (or use the default "MyVPN")
3. Enter the server address for your VPN
4. Select the connection type (PPTP or L2TP/IPSec)
5. Enter your username and password (optional)
6. Check "Save credentials" if you want to save your login information
7. Click "Create VPN" to create the VPN connection
8. Click "Connect" to connect to the VPN
9. Click "Delete VPN" to remove the VPN connection

## Technical Details

The application uses PowerShell commands to create and manage VPN connections:

- `Add-VpnConnection`: Creates a new VPN connection
- `Set-VpnConnection`: Configures VPN properties
- `rasdial`: Connects to the VPN
- `Remove-VpnConnection`: Deletes the VPN connection

Configuration settings are saved to a JSON file in the user's AppData folder.
