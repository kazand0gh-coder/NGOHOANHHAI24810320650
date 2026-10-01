using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TechMartManager
{
    public class MainForm : Form
    {
        // Danh sách dữ liệu & Binding
        private BindingList<Product> _productList = new BindingList<Product>();
        private BindingSource _bindingSource = new BindingSource();
        private List<Category> _categories = new List<Category>();

        // Controls Giao diện
        private TableLayoutPanel mainTableLayout;
        private Panel leftPanel;
        private Panel rightPanel;

        // Form Inputs
        private TextBox txtProductId;
        private TextBox txtProductName;
        private TextBox txtUnitPrice;
        private TextBox txtQuantity;
        private ComboBox cboCategory;
        private PictureBox picAvatar;
        private Button btnChooseImage;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        // Search & DataGridView
        private TextBox txtSearch;
        private DataGridView dgvProducts;

        // Menu & Status Bar
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatusCount;

        // Validation
        private ErrorProvider errorProvider;

        public MainForm()
        {
            InitializeComponentLayout();
            SetupCategories();
            SetupDataBinding();
            RegisterEvents();
            UpdateStatusCount();
        }

        private void InitializeComponentLayout()
        {
            // Cau hinh Form
            this.Text = "TechMart Product Manager";
            this.Size = new Size(1100, 650);
            this.MinimumSize = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterScreen;

            errorProvider = new ErrorProvider();
            errorProvider.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;

            // 1. MenuStrip (Menu Top)
            menuStrip = new MenuStrip();
            ToolStripMenuItem menuFile = new ToolStripMenuItem("File (&F)");
            ToolStripMenuItem menuExportCsv = new ToolStripMenuItem("Export CSV", null, OnExportCsv_Click, Keys.Control | Keys.E);
            ToolStripMenuItem menuExit = new ToolStripMenuItem("Exit", null, (s, e) => Application.Exit(), Keys.Control | Keys.X);
            
            menuFile.DropDownItems.Add(menuExportCsv);
            menuFile.DropDownItems.Add(new ToolStripSeparator());
            menuFile.DropDownItems.Add(menuExit);
            menuStrip.Items.Add(menuFile);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // 2. StatusStrip (Bottom)
            statusStrip = new StatusStrip();
            lblStatusCount = new ToolStripStatusLabel("Tổng số sản phẩm: 0");
            statusStrip.Items.Add(lblStatusCount);
            this.Controls.Add(statusStrip);

            // 3. TableLayoutPanel (TC01: Responsive Layout 35% - 65%)
            mainTableLayout = new TableLayoutPanel();
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.ColumnCount = 2;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainTableLayout.RowCount = 1;
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.Controls.Add(mainTableLayout);
            mainTableLayout.BringToFront();

            // ---------------------------------------------------
            // COT TRAI: KHUNG NHAP LIEU (35%)
            // ---------------------------------------------------
            leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), AutoScroll = true };

            int y = 10;
            leftPanel.Controls.Add(CreateLabel("Mã sản phẩm:", 10, y));
            txtProductId = new TextBox { Location = new Point(110, y), Width = 200, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            leftPanel.Controls.Add(txtProductId);

            y += 35;
            leftPanel.Controls.Add(CreateLabel("Tên sản phẩm:", 10, y));
            txtProductName = new TextBox { Location = new Point(110, y), Width = 200, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            leftPanel.Controls.Add(txtProductName);

            y += 35;
            leftPanel.Controls.Add(CreateLabel("Danh mục:", 10, y));
            cboCategory = new ComboBox { Location = new Point(110, y), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            leftPanel.Controls.Add(cboCategory);

            y += 35;
            leftPanel.Controls.Add(CreateLabel("Đơn giá:", 10, y));
            txtUnitPrice = new TextBox { Location = new Point(110, y), Width = 200, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            leftPanel.Controls.Add(txtUnitPrice);

            y += 35;
            leftPanel.Controls.Add(CreateLabel("Số lượng:", 10, y));
            txtQuantity = new TextBox { Location = new Point(110, y), Width = 200, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            leftPanel.Controls.Add(txtQuantity);

            y += 35;
            leftPanel.Controls.Add(CreateLabel("Ảnh sản phẩm:", 10, y));
            picAvatar = new PictureBox
            {
                Location = new Point(110, y),
                Size = new Size(130, 110),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            leftPanel.Controls.Add(picAvatar);

            btnChooseImage = new Button { Text = "Chọn Ảnh", Location = new Point(245, y), Width = 65, Height = 30 };
            leftPanel.Controls.Add(btnChooseImage);

            y += 120;
            btnAdd = new Button { Text = "Thêm mới", Location = new Point(10, y), Width = 90, Height = 35, BackColor = Color.LightGreen };
            btnUpdate = new Button { Text = "Cập nhật", Location = new Point(110, y), Width = 90, Height = 35, BackColor = Color.LightSkyBlue };
            btnDelete = new Button { Text = "Xóa", Location = new Point(210, y), Width = 90, Height = 35, BackColor = Color.LightCoral };
            leftPanel.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete });

            mainTableLayout.Controls.Add(leftPanel, 0, 0);

            // ---------------------------------------------------
            // COT PHAI: BANG DU LIEU & TIM KIEM (65%)
            // ---------------------------------------------------
            rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            Label lblSearch = new Label { Text = "Tìm kiếm tên SP:", Location = new Point(10, 15), AutoSize = true };
            txtSearch = new TextBox { Location = new Point(120, 12), Width = 250, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            rightPanel.Controls.Add(lblSearch);
            rightPanel.Controls.Add(txtSearch);

            dgvProducts = new DataGridView
            {
                Location = new Point(10, 45),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Size = new Size(640, 480),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            // Định nghĩa cột cho DataGridView (TC03)
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductId", HeaderText = "Mã SP", Width = 80 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "Tên Sản Phẩm", Width = 180 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName", HeaderText = "Danh Mục", Width = 110 });
            
            var colPrice = new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "Đơn Giá", Width = 120 };
            colPrice.DefaultCellStyle.Format = "N0"; // Định dạng 25,000,000 VNĐ
            dgvProducts.Columns.Add(colPrice);

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Số Lượng", Width = 80 });

            rightPanel.Controls.Add(dgvProducts);
            mainTableLayout.Controls.Add(rightPanel, 1, 0);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label { Text = text, Location = new Point(x, y + 3), AutoSize = true };
        }

        private void SetupCategories()
        {
            _categories = new List<Category>
            {
                new Category("CAT01", "Điện thoại"),
                new Category("CAT02", "Laptop"),
                new Category("CAT03", "Phụ kiện")
            };

            cboCategory.DataSource = _categories;
            cboCategory.DisplayMember = "Name";
            cboCategory.ValueMember = "Id";
        }

        private void SetupDataBinding()
        {
            _bindingSource.DataSource = _productList;
            dgvProducts.DataSource = _bindingSource;
        }

        private void RegisterEvents()
        {
            btnChooseImage.Click += OnChooseImage_Click;
            btnAdd.Click += OnAddProduct_Click;
            btnUpdate.Click += OnUpdateProduct_Click;
            btnDelete.Click += OnDeleteProduct_Click;
            dgvProducts.SelectionChanged += OnGridSelectionChanged;
            txtSearch.TextChanged += OnSearchTextChanged;
        }

        // ==========================================
        // XỬ LÝ EVENT & KIỂM THỬ (TEST CASES)
        // ==========================================

        // TC04: Chọn ảnh mở OpenFileDialog
        private void OnChooseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image Files (*.png; *.jpg; *.jpeg)|*.png;*.jpg;*.jpeg|All Files (*.*)|*.*";
                dlg.Title = "Chọn ảnh đại diện sản phẩm";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    picAvatar.ImageLocation = dlg.FileName;
                }
            }
        }

        // TC02 & TC03: Validation ErrorProvider & Thêm sản phẩm
        private void OnAddProduct_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string pId = txtProductId.Text.Trim();
            if (_productList.Any(p => p.ProductId.Equals(pId, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cat = (Category)cboCategory.SelectedItem;
            var product = new Product
            {
                ProductId = pId,
                ProductName = txtProductName.Text.Trim(),
                CategoryId = cat.Id,
                CategoryName = cat.Name,
                UnitPrice = decimal.Parse(txtUnitPrice.Text.Trim()),
                Quantity = int.Parse(txtQuantity.Text.Trim()),
                ImagePath = picAvatar.ImageLocation ?? string.Empty
            };

            _productList.Add(product);
            UpdateStatusCount();
            ClearInputs();
            MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnUpdateProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;
            if (!ValidateInputs()) return;

            Product currentProduct = (Product)dgvProducts.CurrentRow.DataBoundItem;
            var cat = (Category)cboCategory.SelectedItem;

            currentProduct.ProductName = txtProductName.Text.Trim();
            currentProduct.CategoryId = cat.Id;
            currentProduct.CategoryName = cat.Name;
            currentProduct.UnitPrice = decimal.Parse(txtUnitPrice.Text.Trim());
            currentProduct.Quantity = int.Parse(txtQuantity.Text.Trim());
            currentProduct.ImagePath = picAvatar.ImageLocation ?? string.Empty;

            _bindingSource.ResetBindings(false);
            MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // TC05: Kiểm tra Xóa Sản Phẩm với Confirmation Popup
        private void OnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn 1 sản phẩm để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Product selectedProduct = (Product)dgvProducts.CurrentRow.DataBoundItem;

            DialogResult dr = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa sản phẩm \"{selectedProduct.ProductName}\" không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dr == DialogResult.Yes)
            {
                _productList.Remove(selectedProduct);
                UpdateStatusCount();
                ClearInputs();
            }
        }

        // Click chọn dòng trên Grid -> Nạp ngược dữ liệu
        private void OnGridSelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow != null && dgvProducts.CurrentRow.DataBoundItem is Product product)
            {
                txtProductId.Text = product.ProductId;
                txtProductName.Text = product.ProductName;
                txtUnitPrice.Text = product.UnitPrice.ToString("G0");
                txtQuantity.Text = product.Quantity.ToString();
                cboCategory.SelectedValue = product.CategoryId;

                if (!string.IsNullOrEmpty(product.ImagePath) && File.Exists(product.ImagePath))
                {
                    picAvatar.ImageLocation = product.ImagePath;
                }
                else
                {
                    picAvatar.Image = null;
                }
            }
        }

        // Live Search TextChanged
        private void OnSearchTextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword))
            {
                _bindingSource.DataSource = _productList;
            }
            else
            {
                var filtered = _productList
                    .Where(p => p.ProductName.ToLower().Contains(keyword) || p.ProductId.ToLower().Contains(keyword))
                    .ToList();
                _bindingSource.DataSource = new BindingList<Product>(filtered);
            }
        }

        // Xuất file CSV
        private void OnExportCsv_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "CSV Files (*.csv)|*.csv";
                dlg.FileName = "Products_Export.csv";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Mã SP,Tên SP,Danh Mục,Đơn Giá,Số Lượng");

                        foreach (var p in _productList)
                        {
                            sb.AppendLine($"\"{p.ProductId}\",\"{p.ProductName}\",\"{p.CategoryName}\",{p.UnitPrice},{p.Quantity}");
                        }

                        File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Hàm Kiểm tra Validation (TC02)
        private bool ValidateInputs()
        {
            errorProvider.Clear();
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtProductId.Text))
            {
                errorProvider.SetError(txtProductId, "Mã sản phẩm không được để trống!");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                errorProvider.SetError(txtProductName, "Tên sản phẩm không được để trống!");
                isValid = false;
            }

            if (!decimal.TryParse(txtUnitPrice.Text.Trim(), out decimal price) || price <= 0)
            {
                errorProvider.SetError(txtUnitPrice, "Đơn giá phải là số nguyên/thực dương (> 0)!");
                isValid = false;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out int qty) || qty < 0)
            {
                errorProvider.SetError(txtQuantity, "Số lượng phải là số nguyên không âm (≥ 0)!");
                isValid = false;
            }

            return isValid;
        }

        private void UpdateStatusCount()
        {
            lblStatusCount.Text = $"Tổng số sản phẩm: {_productList.Count}";
        }

        private void ClearInputs()
        {
            txtProductId.Clear();
            txtProductName.Clear();
            txtUnitPrice.Clear();
            txtQuantity.Clear();
            picAvatar.Image = null;
            picAvatar.ImageLocation = null;
            errorProvider.Clear();
        }
    }
}