
    $(function () {

            $('#advancedSearchBtn').click(function () {
                $('#advancedSearch').toggle();
            });

            $('#keywords').on('input', function () {
                console.log("change");
            console.log($('#keywords').val());
            $('#searchPhotos').val($('#keywords').val());
            });

            $('#advancedSearchType').change(function () {
                $(".search-type").hide();
            $('#' + $(this).val()).show();
    });

            $(".filter").change(function (event) {

                var filter = $(this).val();
                var filterValue = $(this).find('option:selected').text();
            var filterType = $(event.target);
            var filterType2 = $(this).find('name').val();
            $('#searchPhotos').val(filterValue);
            console.log(filter);
            console.log(filterType);
            console.log(filterType2);
                $.ajax({
                    type: 'POST',
                    url: '/Photos/PhotoAdvancedFilterSearch',
                    data: {"filter": filter, "filterType": filter },
                    success: function (data) {
                        $("#photoList").html(data);
                    }
                    });
                });

});
