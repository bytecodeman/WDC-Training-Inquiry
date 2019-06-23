<%@ Page Language="VB" EnableSessionState="False" EnableViewState="True" AutoEventWireup="false" CodeFile="index.aspx.vb" Inherits="WDC_Training_Inquiry_index" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta charset="utf-8">
    <title>WDC Training Inquiry / Course Waitlist Registration Form</title>
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name="author" content="Antonio C. Silvestri">
    <meta name="robots" content="noindex, nofollow">
    <link rel="stylesheet" href="//stackpath.bootstrapcdn.com/bootstrap/4.2.1/css/bootstrap.min.css" integrity="sha384-GJzZqFGwb1QTTN6wy59ffF1BuGJpLSa9DkKMp0DgiMDm4iYMj70gZWKYbI706tWS" crossorigin="anonymous">
    <style id="View3Style" runat="server" visible="false">
        p.wdcwaitlist a:hover {
            transition: color 0.3s ease-in-out;
            color: #f1eb90;
        }

        p.wdcwaitlist a {
            display: inline-block;
            background-color: #9e1e46;
            color: white;
            padding: 10px 20px;
            font-weight: bold;
            font-size: 1.4rem;
            border-radius: 10px;
            margin: 1.5rem 0;
            text-decoration: none;
        }
    </style>
</head>
<body class="pb-5">
    <div class="container">
        <div class="row">
            <div class="col">
                <img src="https://www.stcc.edu/media/wdc/images/WDCLogo.png" class="img-fluid mx-auto d-block" alt="WDC Logo" />
                <h1 class="my-5 text-center font-weight-bold">Training Inquiry / Course Waitlist Registration Form</h1>
            </div>
        </div>

        <div class="row">
            <div class="col">
                <form id="WDCInquiryForm" runat="server">
                    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                        <asp:View ID="View1" runat="server">
                            <div id="ErrorMsg" runat="server" visible="false" class="alert alert-danger font-weight-bold" role="alert">
                                <asp:Label ID="ErrorText" runat="server" Text="" EnableViewState="False" />
                            </div>
                            <div class="form-group">
                                <label for="Inquiry">Course / Inquiry</label>
                                <asp:TextBox ID="Inquiry" runat="server" MaxLength="120" CssClass="form-control trimText" Text="" ReadOnly="true" />
                            </div>
                            <div class="form-group">
                                <label for="Firstname">First Name</label>
                                <asp:TextBox ID="Firstname" runat="server" MaxLength="40" CssClass="form-control trimText" Text="" />
                            </div>
                            <div class="form-group">
                                <label for="Lastname">Last Name</label>
                                <asp:TextBox ID="Lastname" runat="server" MaxLength="40" CssClass="form-control trimText" Text="" />
                            </div>
                            <div class="form-group">
                                <label for="Phone">Phone</label>
                                <asp:TextBox TextMode="Phone" ID="Phone" runat="server" MaxLength="14" CssClass="form-control trimText" Text="" />
                            </div>
                            <div class="form-group">
                                <label for="Email">Email &nbsp;<span class="text-muted small">(Be sure email is valid.  We cannot contact you otherwise.)</span></label>
                                <asp:TextBox TextMode="Email" ID="Email" runat="server" MaxLength="120" CssClass="form-control trimText" Text="" />
                            </div>
                            <div class="form-group">
                                <label for="Comment">Comment / Question</label>
                                <asp:TextBox TextMode="MultiLine" ID="Comment" runat="server" Rows="3" CssClass="form-control trimText" Text="" />
                            </div>
                            <p>Springfield Technical Community College will use the information you provide on this form to be in touch with you with your query.</p>
                            <div class="form-check">
                                <label class="form-check-label">
                                    <asp:CheckBox ID="Consent" runat="server" CssClass="form-check-input" />
                                    I consent to having this website store my submitted information so they can respond via email to my inquiry.</label>
                            </div>
                            <p class="mt-3">If you change your mind at any time and decide not to be contacted, reach out to us at <a href="mailto:wdc@stcc.edu">wdc@stcc.edu</a>.</p>
                            <p>We will treat your information with respect. For more information about our privacy practices, please visit our <a href="https://www.stcc.edu/about-stcc/consumerinfo/gdpr/" target="_blank">data privacy page</a>.</p>
                            <p class="mt-3">By clicking below, you agree that we may process your information in accordance with these terms.</p>
                            <input type="hidden" name="g-recaptcha-response" id="g-recaptcha-response" value="" />
                            <asp:Button ID="Submit" runat="server" Text="Submit Inquiry" CssClass="btn btn-primary" />
                        </asp:View>
                        <asp:View ID="View2" runat="server">
                            <h2>Your Inquiry / Course Waitlist Registration has been successfully submitted!</h2>
                            <h3 class="my-5">We will contact you as soon as there is news available.</h3>
                            <h3>Thank you,
                                <asp:Label runat="server" ID="ConfirmFirstName" />, for your submission.</h3>
                        </asp:View>
                        <asp:View ID="View3" runat="server">
                            <h2 class="alert alert-danger font-weight-bold">This form was not properly activated!</h2>
                            <h3 class="my-3">Please use this form through pages that have this button:</h3>
                            <p class="wdcwaitlist"><a href="#">Interest / Wait List</a></p>
                        </asp:View>
                    </asp:MultiView>
                </form>
            </div>
        </div>
    </div>
    <script src="//code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
    <script src='//www.google.com/recaptcha/api.js?render=6Lfze4kUAAAAAOaaae8UeCu_OY3hTy4IlULqXPkZ'></script>
    <script>
        $(function () {
            grecaptcha.ready(function () {
                grecaptcha.execute('6Lfze4kUAAAAAOaaae8UeCu_OY3hTy4IlULqXPkZ', { action: 'WDCInquiryForm' })
                    .then(function (token) {
                        $("#g-recaptcha-response").val(token);
                    });
            });

            function toTitleCase(str) {
                return str.replace(
                    /\w\S*/g,
                    function (txt) {
                        return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
                    }
                );
            }

            $("#WDCInquiryForm").submit(function () {
                $(".trimText").each(function () {
                    $(this).val($(this).val().trim());
                });
                $("#Firstname").val(toTitleCase($("#Firstname").val()));
                $("#Lastname").val(toTitleCase($("#Lastname").val()));
                $("#WDCInquiryForm input[type=submit]").click(function () {
                    return false;
                }).val("Please Wait . . .").css({ opacity: 0.2, cursor: "not-allowed" });
                return true;
            });

        });
    </script>
</body>
</html>
