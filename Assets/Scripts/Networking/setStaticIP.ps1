# SetupEthernetForPi.ps1
# This script configures the Ethernet adapter with a static IP and sets up a firewall rule for communication.

# Define the adapter name. Change this if your adapter name is different.
$adapterName = "Ethernet"
# Set the static IP that the adapter will use.
$staticIP = "192.168.2.1"
# Define the subnet mask.
$subnetMask = "255.255.255.0"
# (Optional) Define a default gateway.
$gateway = "192.168.2.1"
# Define DNS servers (optional).
$dns1 = "8.8.8.8"
$dns2 = "8.8.4.4"
# Define the port used for TCP communication with the Pi.
$port = 65432

Write-Host "Configuring network adapter '$adapterName' for Raspberry Pi communication..."

# Set the static IP configuration.
netsh interface ip set address name="$adapterName" static $staticIP $subnetMask $gateway 1

# Set DNS servers.
netsh interface ip set dns name="$adapterName" static $dns1 primary
netsh interface ip add dns name="$adapterName" $dns2 index=2

Write-Host "Network adapter configured with static IP $staticIP."

# Create a firewall rule to allow inbound TCP traffic on the specified port.
Write-Host "Adding firewall rule for TCP port $port..."
New-NetFirewallRule -DisplayName "Allow TCP Port $port" -Direction Inbound -Protocol TCP -LocalPort $port -Action Allow

Write-Host "Setup complete. The adapter is now configured for Raspberry Pi communication."
