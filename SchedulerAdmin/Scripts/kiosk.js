(function ($) {
    var source = $("#kiosks-template").html();
    var template = Handlebars.compile(source);

    $.fn.kiosk = function (options) {
        return this.each(function () {
            var $this = $(this);

            var opts = $.extend({}, { apiUrl: "api/", onInit: null }, options, $this.data());

            var getKiosks = function () {
                var def = $.Deferred();

                return $.ajax({
                    "url": opts.apiUrl + "kiosk"
                }).done(function (data) {
                    var context = { "kiosks": data };
                    var html = template(context);
                    $(".panel-body", $this).html(html);
                    def.resolve();
                }).fail(def.reject);

                return def.promise();
            }

            getKiosks().done(function () {
                if (typeof opts.onInit == "function")
                    opts.onInit.call($this);
            });

            $this.on("click", ".command-link", function (e) {
                e.preventDefault();

                var cell = $(this).closest("td");
                var row = $(this).closest("tr");

                var args = $(this).data();

                if (args.command == "edit") {
                    $(".edit-delete", cell).hide();
                    $(".save-cancel", cell).show();

                    row.data("orig-kiosk-name", cell.find("td:eq(0)").html());

                } else if (args.command == "cancel") {
                    $(".edit-delete", cell).show();
                    $(".save-cancel", cell).hide();
                }
                
            });
        });
    }
}(jQuery));