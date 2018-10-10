
    $(function () {

            $('#advancedSearchBtn').click(function () {
                $('#advancedSearch').toggle();
            });

            $('#keywords').on('input', function () {
                console.log("change");
            console.log($('#keywords').val());
            $('#searchPhotos').val($('#keywords').val());
            });

            //$('#advancedSearchType').change(function () {
            //    $(".search-type").hide();
            //$('#' + $(this).val()).show();
    //});

            $(".filter").change(function () {

                var filter = $(this).val();
                var filterValue = $(this).find('option:selected').text();
                var filterType = this.id;
            $('#searchPhotos').val(filterValue);
                $.ajax({
                    type: 'POST',
                    url: '/Photos/PhotoAdvancedFilterSearch',
                    data: { "filter": filterValue, "filterType": filterType },
                    success: function (data) {
                        $("#photoList").html(data);
                    }
                    });
                });

});
