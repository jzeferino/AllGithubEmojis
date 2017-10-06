$(document).ready(function(e) {

var availableTags = [];
	// READ JSON
	$.getJSON("emojis.json", function(json) {
		
		// LOOP GROUPS
	    $.each(json.Groups, function (idx, group) {
	    	var groupElement = document.createElement("div");
	    		$(groupElement).addClass("group");
	    		$(groupElement).addClass("row");
	    		$(groupElement).addClass("col l12");
	    		$(groupElement).html("<h1 class='flow-text'>" + group.Name + "</h1>");
	    	
			// LOOP SUBGROUPS
			$.each(group.SubGroups, function (idx, subGroup) {
				var subGroupElement = document.createElement("div");
	    		$(subGroupElement).addClass("sub-group");
	    		$(subGroupElement).addClass("row");
	    		$(subGroupElement).addClass("card");
	    		$(subGroupElement).addClass("col l12");
				$(subGroupElement).html("<h2 class='card-title flow-text'>" + subGroup.Name + "</h2>");
	    		$(groupElement).append(subGroupElement);

				// LOOP EMOJI
	    		$.each(subGroup.Emojis, function (idx, emoji) {
	    			var emojiElement = document.createElement("div");
	    			$(emojiElement).addClass("emoji");
	    			$(emojiElement).addClass("col l2");
	    			$(emojiElement).addClass("col m2");
					$(emojiElement).addClass("col s6");
					
	    			availableTags.push(emoji.Name);

	    			$(emojiElement).html("<span class='flow-text'>" + emoji.Name + "</span><div ><img alt='" + emoji.Name + "' title='Click to copy to clipboard' src='" + emoji.Url + "' /><p>:" + emoji.Code + ":</p></div>");
	    			$(subGroupElement).append(emojiElement);
	    		});

	    	});

			$("#main").append(groupElement);
	    });
	});

	var copyCode = new Clipboard('.emoji', {
	    text: function(trigger) {
	        return $(trigger).find("p").html();
	    }
	});

	 $("#search-field").autocomplete({
      source: availableTags,
      select: function(e, ui) {
      	$("#search-field").val(ui.item.value).trigger("input");
      }
    });

	$("#search-field").on("input", function(e) {
		var search = $(this).val();
		if (search != "")
		{
			$("div.emoji span:not(" + search + ")").closest("div.emoji").addClass("hidden");
			$("div.emoji span:contains(" + search + ")").closest("div.emoji").removeClass("hidden");
			
		} else {
			$("div.emoji").removeClass("hidden");
		}

		$.each($(".sub-group"), function (idx, subGroup) {
	    		if ($(subGroup).find(".emoji").length == $(subGroup).find(".emoji.hidden").length)
	    		{
	    			$(subGroup).addClass("hidden");
	    		} else {
	    			$(subGroup).removeClass("hidden");
	    		}
    	});

		$.each($(".group"), function (idx, group) {
				$(group).find(".no-results").remove();
	    		if ($(group).find(".sub-group").length == $(group).find(".sub-group.hidden").length)
	    		{
	    			$(group).append("<p class='no-results'>No results found</p>");
	    		}
    	});

	});

	$("#scroll-top").on("click", function(e) {
	    document.body.scrollTop = 0; // For Chrome, Safari and Opera 
	    document.documentElement.scrollTop = 0; // For IE and Firefox
	});

	window.onscroll = function() {scrollFunction()};

	function scrollFunction() {
	    if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
	        document.getElementById("scroll-top").style.display = "block";
	    } else {
	        document.getElementById("scroll-top").style.display = "none";
	    }
	}
});

$("body").on("scroll", function(e) {
  if (this.scrollTop > 147) {
    wrap.addClass("fix-search");
  } else {
    wrap.removeClass("fix-search");
  }
});