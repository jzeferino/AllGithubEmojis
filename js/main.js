$(document).ready(function (e) {
  var availableTags = []

  // READ JSON
  $.getJSON('emojis.json', { async: true }, function (json) {
    document.getElementById('main-content').style.display = 'block'
    document.getElementById('loading').style.display = 'none'

    // LOOP GROUPS
    Array.prototype.forEach.call(json.Groups, function (group) {
      var groupElement = document.createElement('div')

      groupElement.className = 'group row col s12'
      groupElement.innerHTML = "<h1 class='flow-text'>" + group.Name + '</h1>'

      Array.prototype.forEach.call(group.SubGroups, function (subGroup) {
        var subGroupElement = document.createElement('div')

        subGroupElement.className = 'sub-group row card col s12'
        subGroupElement.innerHTML = "<h2 class='card-title flow-text'>" + subGroup.Name + '</h2>'

        Array.prototype.forEach.call(subGroup.Emojis, function (emoji) {
          var emojiElement = document.createElement('div')

          emojiElement.className = 'emoji col l2 m2 s6'
          emojiElement.innerHTML = "<span class='flow-text' title='" + emoji.Name.toLocaleUpperCase() + "'>" + emoji.Name + "</span><div><img alt='" + emoji.Name + "' title='Click to copy to clipboard' src='" + emoji.Url + "' /><p>:" + emoji.Code + ':</p></div>'

          availableTags.push(emoji.Name)

          subGroupElement.append(emojiElement)
        })

        groupElement.append(subGroupElement)
      })

      document.getElementById('main').append(groupElement)

      $('.page-footer').removeClass('loading')
    })

    var copyCode = new Clipboard('.emoji', {
      text: function (trigger) {
        return $(trigger).find('p').html()
      }
    })

    var hideEmptySection = function (selector, callback) {
      var arraySections = $(selector).map(function (index, element) {
        return $(element).find('.emoji.hidden').length === $(element).find('.emoji').length
      })

      $.each(arraySections, function (index, value) {
        if (value) {
          $(selector).eq(index).addClass('hidden')
        }
      })

      if (callback !== undefined) {
        callback(arraySections)
      }
    }

    $('body').on('click', '.emoji', function (e) {
      Materialize.toast('Copied to clipboard!', 1000)
    })

    $('#search-field').autocomplete({
      source: availableTags,
      select: function (e, ui) {
        $('#search-field').val(ui.item.value).trigger('input')
      }
    })

    var idTimer

    $('#search-field').on('input', function () {
      var search = $(this).val()

      clearTimeout(idTimer)

      idTimer = setTimeout(function () {
        $('.sub-group, .group').removeClass('hidden')
        $('#main').find('.no-results').remove()

        if (search !== '') {
          search = search.normalize('NFD').replace(/[\u0300-\u036f]/g, '')
          search = search.toLocaleLowerCase()

          $('div.emoji span:not(' + search + ')').closest('div.emoji').addClass('hidden')
          $('div.emoji span:contains(' + search + ')').closest('div.emoji').removeClass('hidden')

          hideEmptySection('.sub-group')
          hideEmptySection('.group', function (arraySections) {
            if (arraySections.toArray().every(function (e) { return e })) {
              $('#main').append("<p class='no-results'>No results found</p>")
            }
          })
        } else {
          $('.emoji').removeClass('hidden')
        }
      }, 200)
    })
  })
})

$('body').on('click', '#scroll-top', function (e) {
  $('html, body').animate({ scrollTop: 0 }, 100)

  return false
})

$('body').on('scroll', function (e) {
  if (document.body.scrollTop > 147) {
    $('body').addClass('fix-search')
  } else {
    $('body').removeClass('fix-search')
  }

  if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
    document.getElementById('scroll-top').style.display = 'block'
  } else {
    document.getElementById('scroll-top').style.display = 'none'
  }
})
