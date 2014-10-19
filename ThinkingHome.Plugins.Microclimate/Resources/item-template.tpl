<div class="microclimate-sensor-actions"></div>
<div class="microclimate-sensor-name">
	<%= displayName %>
</div>

<% if (current) { %>

		<div class="microclimate-sensor-data">
			<%= current.dt %>
		</div>
		<div>
			<%= current.dd %>
		</div>
<% } else { %>

	<div class="microclimate-sensor-no-data">
		The sensor has no data
	</div>
<% } %>

