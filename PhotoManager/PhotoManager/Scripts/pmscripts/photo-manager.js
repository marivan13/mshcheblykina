
$(function () {

          
            //open/close advanced search filter
            $('#advancedSearchBtn').click(function () {
                $('#advancedSearch').toggle();
            });

            // check if click button with empty search field
                $('#searchBtn').click(function (e) {
                    var searchString = $('#searchField').val();
                if (searchString == '') {
                   // e.preventDefault();
                    $.ajax({
                        type: 'GET',
                        url: '/Photos/GetAllPhotosForUser',
                        success: function (data) {
                            $("#photoList").html(data);
                        }
                    });
                }
                else {
                    $.ajax({
                        type: 'POST',
                        url: '/Photos/PhotoAdvancedSearch',
                        data: { "searchField": searchString },
                        success: function (data) {
                            $("#photoList").html(data);
                        }
                    });
                }
                });


                $('#keywords').on('input', function () {
                    $('#searchField').val($('#keywords').val());
                });

            //reset filters value
                $('#clearFilter').click(function () {
                $(".filter").each(function () {
                    console.log(this)
                    $(this).find('option:first').attr('selected', 'selected');
                    $('#searchField').val('');
                    });
                    $.ajax({
                        type: 'GET',
                        url: '/Photos/GetAllPhotosForUser',
                        success: function (data) {
                            $("#photoList").html(data);
                        }
                    });
                });

                
            //advanced search by filters
                $(".filter").change(function () {
                var searchString = '';
                var filter = {};
                $(".filter").each(function () {
                    var filterValue = $(this).find('option:selected').text();
                    var filterType = this.id;
                    filter[filterType] = filterValue;
                    if (filterValue != 'All') {
                        searchString += filterValue;
                        $('#searchField').val(searchString);
                    }
                });
                $('#searchPhotos').val(filter);
                    $.ajax({
                        type: 'POST',
                        url: '/Photos/PhotoAdvancedFilterSearch',
                        data: { "filter": filter},
                        success: function (data) {
                            $("#photoList").html(data);
                        }
                        });
                    });

});
