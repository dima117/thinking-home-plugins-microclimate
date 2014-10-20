<div class="microclimate-sensor-actions"></div>
<div class="microclimate-sensor-name">
	<%= displayName %>
</div>

<% if (data) { %>

		<div class="microclimate-sensor-data">
			<%= data.dt %>
		</div>
		<div>
			<%= data.dd %>
		</div>
<% } else { %>

	<div class="microclimate-sensor-no-data">
		The sensor has no data
	</div>
<% } %>

