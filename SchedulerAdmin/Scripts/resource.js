(function ($) {
    $.fn.resource = function (options) {
        return this.each(function () {
            var $this = $(this);

            var opts = $.extend({}, { apiUrl: "api/", resourceId: 0, returnTo: "resources", onInit: null, onSave: null }, options, $this.data());

            function LnfApi(url) {
                var apiGet = function (path, args) {
                    var def = $.Deferred();

                    $.ajax({
                        "url": url + path,
                        "method": "GET",
                        "data": args
                    }).done(function (data, textStatus, jqXHR) {
                        def.resolve(data);
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        def.reject(jqXHR.responseJSON);
                    });

                    return def.promise();
                };

                var apiPost = function (path, args) {
                    var def = $.Deferred();

                    $.ajax({
                        "url": url + path,
                        "method": "POST",
                        "data": args
                    }).done(function (data, textStatus, jqXHR) {
                        def.resolve(data);
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        def.reject(jqXHR.responseJSON);
                    });

                    return def.promise();
                };

                this.getGracePeriodMinutes = function (granularity, minReservTime, gracePeriodHour) {
                    return apiGet("resource/grace-period-minute", { "granularity": granularity, "minReservTime": minReservTime, "gracePeriodHour": gracePeriodHour });
                };

                this.getGracePeriodHours = function (granularity, minReservTime) {
                    return apiGet("resource/grace-period-hour", { "granularity": granularity, "minReservTime": minReservTime });
                }

                this.getMaxReservationTimes = function (granularity, minReservTime) {
                    return apiGet("resource/max-reservation-time", { "granularity": granularity, "minReservTime": minReservTime });
                }

                this.getMinReservationTimes = function (granularity) {
                    return apiGet("resource/min-reservation-time", { "granularity": granularity });
                };

                this.getOffsets = function (granularity) {
                    return apiGet("resource/offset", { "granularity": granularity });
                }

                this.getProcessTechs = function () {
                    return apiGet("proctech");
                }

                this.getResource = function (resourceId) {
                    return apiGet("resource/" + resourceId);
                };

                this.saveResource = function (model) {
                    return apiPost("resource", model);
                }
            }

            var lnfapi = new LnfApi(opts.apiUrl);

            var loadGracePeriodMinutes = function (items, selectedValue) {
                $(".grace-period-minute", $this).html($.map(items, function (item, index) {
                    return $("<option/>", { "value": item }).text(item).prop("selected", item == selectedValue);
                }));
            }

            var loadGracePeriodHours = function (items, selectedValue) {
                $(".grace-period-hour", $this).html($.map(items, function (item, index) {
                    return $("<option/>", { "value": item }).text(item).prop("selected", item == selectedValue);
                }));
            }

            var loadMaxReservationTimes = function (items, selectedValue) {
                $(".max-reservation", $this).html($.map(items, function (item, index) {
                    return $("<option/>", { "value": item }).text(item).prop("selected", item == selectedValue);
                }));
            }

            var loadMinReservationTimes = function (items, selectedValue) {
                $(".min-reservation", $this).html($.map(items, function (item, index) {
                    return $("<option/>", { "value": item.Value }).text(item.Text).prop("selected", item.Value == selectedValue);
                }));
            }

            var loadOffsets = function (items, selectedValue) {
                $(".offset", $this).html($.map(items, function (item, index) {
                    return $("<option/>", { "value": item }).text(item).prop("selected", item == selectedValue);
                }));
            }

            var loadProcessTechs = function (items, selectedValue) {
                $(".process-tech", $this).html($.map(items, function (item, index) {
                    var name = item.BuildingName + " / " + item.LabDisplayName + " / " + item.ProcessTechName;
                    return $("<option/>", { "value": item.ProcessTechID }).text(name).prop("selected", item.ProcessTechID == selectedValue);
                }));
            };

            var loadResource = function (res) {
                if (res.ResourceID == 0) {
                    $(".panel-title", $this).html("Add New Resource");
                } else {
                    $(".panel-title", $this).html("Edit Resource: " + res.ResourceName);
                }

                $(".resource-name", $this).val(res.ResourceName);
                $(".resource-description", $this).val(res.ResourceDescription);
                $(".auth-duration", $this).val(res.AuthDuration);
                $(".rolling-period", $this).prop("checked", res.RollingPeriod);
                $(".reservation-fence", $this).val(res.ReservationFenceHours);
                $(".granularity", $this).val(res.GranularityMinutes);
                $(".max-schedulable", $this).val(res.MaxSchedulableHours);
                $(".min-cancel-time", $this).val(res.MinCancelTimeMinutes);
                $(".auto-end", $this).val(res.AutoEndMinutes);
                $(".unload-time", $this).val(res.UnloadTimeMinutes);
                $(".helpdesk-email", $this).val(res.HelpdeskEmail);
                $(".wiki-page-url", $this).val(res.WikiPageUrl);
                $(".is-schedulable", $this).prop("checked", res.IsSchedulable);
                $(".is-active", $this).prop("checked", res.IsActive);
            }

            var getModel = function () {
                return {
                    ResourceID: $(".resource-id", $this).val(),
                    ResourceName: $(".resource-name", $this).val(),
                    ResourceDescription: $(".resource-description", $this).val(),
                    ProcessTechID: $(".process-tech", $this).val(),
                    AuthDuration: $(".auth-duration", $this).val(),
                    RollingPeriod: $(".rolling-period", $this).prop("checked"),
                    ReservationFenceHours: $(".reservation-fence", $this).val(),
                    GranularityMinutes: $(".granularity", $this).val(),
                    OffsetHours: $(".offset", $this).val(),
                    MinReservationMinutes: $(".min-reservation", $this).val(),
                    MaxReservationHours: $(".max-reservation", $this).val(),
                    MaxSchedulableHours: $(".max-schedulable", $this).val(),
                    MinCancelTimeMinutes: $(".min-cancel-time", $this).val(),
                    GracePeriodHours: $(".grace-period-hour", $this).val(),
                    GracePeriodMinutes: $(".grace-period-minute", $this).val(),
                    AutoEndMinutes: $(".auto-end", $this).val(),
                    UnloadTimeMinutes: $(".unload-time", $this).val(),
                    HelpdeskEmail: $(".helpdesk-email", $this).val(),
                    WikiPageUrl: $(".wiki-page-url", $this).val(),
                    IsSchedulable: $(".is-schedulable", $this).prop("checked"),
                    IsActive: $(".is-active", $this).prop("checked")
                };
            }

            var init = function (res) {
                $this.on("change", ".granularity", function (e) {
                    var gran = $(this).val();

                    lnfapi.getOffsets(gran).done(function (offsets) {
                        loadOffsets(offsets, res.OffsetHours);
                        lnfapi.getMinReservationTimes(gran).done(function (minReservTimes) {
                            loadMinReservationTimes(minReservTimes, res.MinReservationMinutes);
                            var minReservTime = $(".min-reservation", $this).val();
                            lnfapi.getMaxReservationTimes(gran, minReservTime).done(function (maxReservTimes) {
                                loadMaxReservationTimes(maxReservTimes, res.MaxReservationHours);
                                lnfapi.getGracePeriodHours(gran, minReservTime).done(function (gpHours) {
                                    loadGracePeriodHours(gpHours, res.GracePeriodHours);
                                    var gracePeriodHour = $(".grace-period-hour", $this).val();
                                    lnfapi.getGracePeriodMinutes(gran, minReservTime, gracePeriodHour).done(function (gpMinutes) {
                                        loadGracePeriodMinutes(gpMinutes, res.GracePeriodMinutes);
                                    });
                                });
                            });
                        });
                    });
                }).on("change", ".min-reservation", function (e) {
                    var gran = $(".granularity", $this).val();
                    var minReservTime = $(this).val();

                    lnfapi.getMaxReservationTimes(gran, minReservTime).done(function (maxReservTimes) {
                        loadMaxReservationTimes(maxReservTimes, res.MaxReservationHours);
                        lnfapi.getGracePeriodHours(gran, minReservTime).done(function (gpHours) {
                            loadGracePeriodHours(gpHours, res.GracePeriodHours);
                            var gracePeriodHour = $(".grace-period-hour", $this).val();
                            lnfapi.getGracePeriodMinutes(gran, minReservTime, gracePeriodHour).done(function (gpMinutes) {
                                loadGracePeriodMinutes(gpMinutes, res.GracePeriodMinutes);
                            });
                        });
                    });
                }).on("change", ".grace-period-hour", function (e) {
                    var gran = $(".granularity", $this).val();
                    var minReservTime = $(".min-reservation", $this).val();
                    var gracePeriodHour = $(this).val();

                    lnfapi.getGracePeriodMinutes(gran, minReservTime, gracePeriodHour).done(function (gpMinutes) {
                        loadGracePeriodMinutes(gpMinutes, res.GracePeriodMinutes);
                    });
                }).on("click", "[data-command]", function (e) {
                    var command = $(this).data("command");

                    var model = getModel();

                    lnfapi.saveResource(model).done(function () {
                        if (command == "SaveAndReturn")
                            window.location = opts.returnTo;
                    });

                    if (typeof opts.onSave == "function")
                        opts.onSave.call($this);
                });

                $this.show();

                if (typeof opts.onInit == "function")
                    opts.onInit.call($this);
            }

            lnfapi.getResource(opts.resourceId).done(function (res) {
                loadResource(res);
                lnfapi.getProcessTechs().done(function (procTechs) {
                    loadProcessTechs(procTechs, res.ProcessTechID);
                    lnfapi.getOffsets(res.GranularityMinutes).done(function (offsets) {
                        loadOffsets(offsets, res.OffsetHours);
                        lnfapi.getMinReservationTimes(res.GranularityMinutes).done(function (minReservTimes) {
                            loadMinReservationTimes(minReservTimes, res.MinReservationMinutes);
                            lnfapi.getMaxReservationTimes(res.GranularityMinutes, res.MinReservationMinutes).done(function (maxReservTimes) {
                                loadMaxReservationTimes(maxReservTimes, res.MaxReservationHours);
                                lnfapi.getGracePeriodHours(res.GranularityMinutes, res.MinReservationMinutes).done(function (gpHours) {
                                    loadGracePeriodHours(gpHours, res.GracePeriodHours);
                                    lnfapi.getGracePeriodMinutes(res.GranularityMinutes, res.MinReservationMinutes, res.GracePeriodHours).done(function (gpMinutes) {
                                        loadGracePeriodMinutes(gpMinutes, res.GracePeriodMinutes);
                                        init(res);
                                    });
                                });
                            });
                        });
                    });
                });
            });

        });
    };
})(jQuery)