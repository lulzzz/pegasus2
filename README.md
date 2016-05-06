# Create README.md file for PegasusMission

A README.md file is intended to quickly orient readers to what your project can do. [Learn more](http://go.microsoft.com/fwlink/p/?LinkId=524306) about Markdown.
This project b

## Get started with this page
 1. Edit the contents of this page
 2. Commit changes

## Use Visual Studio
 1. Connect with Team Explorer
 2. Clone repository
 3. Add a README.md file
 4. Commit and push changes

## Use command line
Use the following commands inside the folder of your repository:

 1. git add README.md
 2. git commit -m "Adding project documentation" 
 3. git push origin master



The Pegasus mission is all about experimentation and innovation and involves both a high altitude balloon (HAB) scientific payload and the “Internet of Things”, aka IOT.  The following are the mission goals and technical objectives.

Mission Goals
 (1) In-flight telemetry
 (2) A photo of the curvature of earth, edge of atmosphere, and blackness of space.

Technical Objectives
 (1) Real-time telemetry
 (2) Broadcast of telemetry
 (3) Telemetry capture
 (4) Real-time command and control

The idea behind Pegasus is to perform real-time IOT in the hostile environment of near space.  The definition of “real-time” requires a deadline and for this mission we set the bar at < 100ms from the edge of near space to an observer, e.g., a phone or Web site.  We will capture the telemetry in cloud storage accounts for later analysis, but allow the observers to participant in the experiment in anywhere in the world as it is happening.

Telemetry
 (1) Ground speed in knots
 (2) Heading – compass point of the direction the payload is traveling
 (3) Camera Angle – compass point of the direction of the camera
 (4) GPS – latitude and longitude of the payload
 (5) Pressure – Barometric pressure of the atmosphere
 (6) Internal Temp – Internal temperature of the payload
 (7) External Temp – External temperature of the atmosphere
 (8) Humidity – Atmospheric humidity
 (9) Altitude – Altitude of the payload
 (10) Signal Strength – Strength of transmitters
 (11) Battery – The charge remaining in the battery
 (12) Accelerometer – 3D force on the payload (x,y,z)

This is a total of 17 data points streaming every second from the payload.

Commands
 Commands are used to communicate back to the payload to perform functions associated with cut-down (releasing the balloon tether from the payload to begin the descent stage) as well as parachute deployment commands.

Ascent
 The ascent stage is slightly over 1 hour for Pegasus-1, which should get to about 22,000 meters, which is enough to meet Pegasus-1 mission objectives.  During ascent we will be busy checking the systems for nominal condition.  More on this later.

Descent
 The descent stage is all the drama Pegasus-2 makes a controlled HALO descent to 1,000 meters before deploying its main parachute.  The entire descent should take only 10-12 minutes.  It’s really “Fly or Die” time and the onboard and controls systems must work perfectly.

That is the summary for Pegasus-2 with more to follow on the aeronautics, payload, and operational technology that enables the real-time aspects of the mission. 
