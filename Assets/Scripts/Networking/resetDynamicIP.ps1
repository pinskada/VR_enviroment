# ResetEthernet.ps1
# This script reverts the Ethernet adapter back to DHCP mode and removes the custom firewall rule.

# Define the adapter name.
$adapterName = "Ethernet"
# The port used previously for TCP communication.
$port = 65432

Write-Host "Resetting network adapter '$adapterName' to default DHCP settings..."

# Change the IP configuration to DHCP.
netsh interface ip set address name="$adapterName" source=dhcp

# Revert DNS settings to DHCP.
netsh interface ip set dns name="$adapterName" source=dhcp

Write-Host "Network adapter reset to DHCP."

# Remove the firewall rule that was added for the TCP port.
Write-Host "Removing firewall rule for TCP port $port..."
netsh advfirewall firewall delete rule name="Allow TCP Port $port"

Write-Host "Ethernet adapter configuration has been reset for normal internet use."
