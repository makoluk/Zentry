// API Configuration - Use Railway backend
const API_BASE_URL = 'https://zentry-production.up.railway.app/api/v1';

// Global state
let categories = [];
let habits = [];
let currentCategoryId = null;
let currentTaskId = null;
let currentHabitId = null;
let isEditMode = false;
let draggedCategoryId = null;
let draggedTaskId = null;
let currentPage = 'tasks'; // 'tasks' or 'habits'
let currentWeekStart = new Date();

// Touch/Mobile drag & drop state
let touchStartPos = { x: 0, y: 0 };
let isDragging = false;
let draggedElement = null;
let touchDragType = null; // 'category' or 'task'

// DOM Elements
const categoriesGrid = document.getElementById('categoriesGrid');
const loadingState = document.getElementById('loadingState');
const emptyState = document.getElementById('emptyState');
const categoryModal = document.getElementById('categoryModal');
const taskModal = document.getElementById('taskModal');
const categoryForm = document.getElementById('categoryForm');
const taskForm = document.getElementById('taskForm');

// Page elements
const tasksPage = document.getElementById('tasksPage');
const habitsPage = document.getElementById('habitsPage');
const tasksTab = document.getElementById('tasksTab');
const habitsTab = document.getElementById('habitsTab');
const habitsTable = document.getElementById('habitsTable');

// Utility Functions
const showLoading = () => {
    loadingState.classList.remove('hidden');
    categoriesGrid.classList.add('hidden');
    emptyState.classList.add('hidden');
};

const hideLoading = () => {
    loadingState.classList.add('hidden');
};

const showEmpty = () => {
    emptyState.classList.remove('hidden');
    categoriesGrid.classList.add('hidden');
};

const showCategories = () => {
    categoriesGrid.classList.remove('hidden');
    emptyState.classList.add('hidden');
};

const showModal = (modal) => {
    modal.classList.remove('hidden');
    document.body.style.overflow = 'hidden';
};

const hideModal = (modal) => {
    modal.classList.add('hidden');
    document.body.style.overflow = '';
};

const showToast = (message, type = 'success') => {
    const toast = document.createElement('div');
    toast.className = `fixed top-4 right-4 px-6 py-3 rounded-lg text-white z-50 transition-all transform translate-x-full ${
        type === 'success' ? 'bg-green-500' : 'bg-red-500'
    }`;
    toast.textContent = message;
    document.body.appendChild(toast);
    
    setTimeout(() => toast.classList.remove('translate-x-full'), 100);
    setTimeout(() => {
        toast.classList.add('translate-x-full');
        setTimeout(() => document.body.removeChild(toast), 300);
    }, 3000);
};

// API Functions
const apiCall = async (endpoint, options = {}) => {
    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers
            },
            ...options
        });

        // Check if response is empty (204 No Content)
        if (response.status === 204) {
            return null;
        }

        if (!response.ok) {
            let errorMessage = `HTTP error! status: ${response.status}`;
            try {
                const errorData = await response.text();
                if (errorData) {
                    const parsedError = JSON.parse(errorData);
                    errorMessage = parsedError.message || errorMessage;
                }
            } catch (parseError) {
                console.warn('Could not parse error response:', parseError);
            }
            showToast(`Hata: ${errorMessage}`, 'error');
            throw new Error(errorMessage);
        }

        const responseText = await response.text();
        if (!responseText) {
            return null;
        }

        try {
            const result = JSON.parse(responseText);
            return result.data || result;
        } catch (parseError) {
            console.error('JSON Parse Error:', parseError);
            console.error('Response text:', responseText);
            showToast(`JSON parse hatası: ${parseError.message}`, 'error');
            throw new Error(`Failed to parse JSON response: ${parseError.message}`);
        }
    } catch (error) {
        console.error('API Error:', error);
        if (!error.message.includes('Hata:')) {
            showToast(`Hata: ${error.message}`, 'error');
        }
        throw error;
    }
};

const loadCategories = async () => {
    try {
        showLoading();
        categories = await apiCall('/Categories');
        renderCategories();
    } catch (error) {
        console.error('Failed to load categories:', error);
    } finally {
        hideLoading();
    }
};

const createCategory = async (categoryData) => {
    return await apiCall('/Categories', {
        method: 'POST',
        body: JSON.stringify(categoryData)
    });
};

const updateCategory = async (id, categoryData) => {
    return await apiCall(`/Categories/${id}`, {
        method: 'PUT',
        body: JSON.stringify(categoryData)
    });
};

const deleteCategory = async (id) => {
    return await apiCall(`/Categories/${id}`, {
        method: 'DELETE'
    });
};

const loadTasks = async (categoryId) => {
    return await apiCall(`/Tasks?categoryId=${categoryId}`);
};

const createTask = async (taskData) => {
    return await apiCall('/Tasks', {
        method: 'POST',
        body: JSON.stringify(taskData)
    });
};

const updateTask = async (id, taskData) => {
    return await apiCall(`/Tasks/${id}`, {
        method: 'PUT',
        body: JSON.stringify(taskData)
    });
};

const toggleTask = async (id) => {
    return await apiCall(`/Tasks/${id}/toggle`, {
        method: 'PATCH'
    });
};

const deleteTask = async (id) => {
    return await apiCall(`/Tasks/${id}`, {
        method: 'DELETE'
    });
};

// Render Functions
const getIconEmoji = (icon) => {
    const iconMap = {
        'folder': '📁',
        'star': '⭐',
        'heart': '❤️',
        'check': '✅',
        'flag': '🚩',
        'target': '🎯',
        'droplets': '💧',
        'activity': '🏃',
        'book': '📚',
        'brain': '🧠',
        'moon': '🌙',
        'coffee': '☕'
    };
    return iconMap[icon] || '📁';
};

// Get habit icon based on habit name/type
const getHabitIcon = (habitName, icon) => {
    const name = habitName.toLowerCase();
    
    if (name.includes('egzersiz') || name.includes('spor') || name.includes('koşu') || name.includes('fitness')) {
        return 'activity';
    } else if (name.includes('su') || name.includes('içmek') || name.includes('hidrasyon')) {
        return 'droplets';
    } else if (name.includes('kitap') || name.includes('okuma') || name.includes('okumak')) {
        return 'book';
    } else if (name.includes('meditasyon') || name.includes('mindfulness') || name.includes('nefes')) {
        return 'brain';
    } else if (name.includes('uyku') || name.includes('sleep') || name.includes('dinlenme')) {
        return 'moon';
    } else if (name.includes('kahve') || name.includes('çay') || name.includes('içecek')) {
        return 'coffee';
    }
    
    return icon || 'target';
};

// Check if a date is today
const isToday = (date) => {
    const today = new Date();
    return date.toDateString() === today.toDateString();
};

// Color mapping for categories
const colorMap = {
    amber: 'bg-amber-100 text-amber-600 ring-amber-200',
    sky: 'bg-sky-100 text-sky-600 ring-sky-200',
    rose: 'bg-rose-100 text-rose-600 ring-rose-200',
    emerald: 'bg-emerald-100 text-emerald-600 ring-emerald-200',
    violet: 'bg-violet-100 text-violet-600 ring-violet-200'
};

// Get category color class based on color prop or default
const getCategoryColorClass = (color) => {
    // Map hex colors to predefined color names
    const colorMapping = {
        '#F59E0B': 'amber',
        '#0EA5E9': 'sky',
        '#F43F5E': 'rose',
        '#10B981': 'emerald',
        '#8B5CF6': 'violet'
    };
    
    const colorName = colorMapping[color] || 'amber';
    return colorMap[colorName];
};

// Get habit color class based on color prop or default
const getHabitColorClass = (color) => {
    // Map hex colors to predefined color names for habits
    const colorMapping = {
        '#3B82F6': 'bg-blue-100 text-blue-600',
        '#EF4444': 'bg-red-100 text-red-600',
        '#10B981': 'bg-green-100 text-green-600',
        '#8B5CF6': 'bg-purple-100 text-purple-600',
        '#F59E0B': 'bg-yellow-100 text-yellow-600',
        '#EC4899': 'bg-pink-100 text-pink-600'
    };
    
    return colorMapping[color] || 'bg-indigo-100 text-indigo-600';
};

// Get habit row color class based on color prop or default
const getHabitRowColorClass = (color) => {
    // Map hex colors to predefined color names for habit rows
    const colorMapping = {
        '#3B82F6': 'bg-blue-50 border-l-4 border-blue-500',
        '#EF4444': 'bg-red-50 border-l-4 border-red-500',
        '#10B981': 'bg-green-50 border-l-4 border-green-500',
        '#8B5CF6': 'bg-purple-50 border-l-4 border-purple-500',
        '#F59E0B': 'bg-yellow-50 border-l-4 border-yellow-500',
        '#EC4899': 'bg-pink-50 border-l-4 border-pink-500'
    };
    
    return colorMapping[color] || 'bg-indigo-50 border-l-4 border-indigo-500';
};

// Get badge color based on task count and overdue status
const getBadgeColor = (taskCount, overdueCount = 0) => {
    if (overdueCount > 0) {
        return 'bg-rose-100 text-rose-700';
    } else if (taskCount > 0) {
        return 'bg-emerald-100 text-emerald-700';
    } else {
        return 'bg-gray-100 text-gray-600';
    }
};

const renderCategories = () => {
    if (categories.length === 0) {
        showEmpty();
        return;
    }

    showCategories();
    categoriesGrid.innerHTML = categories.map(category => {
        const colorClass = getCategoryColorClass(category.color);
        const badgeColor = getBadgeColor(category.taskCount || 0, category.overdueCount || 0);
        
        return `
        <div class="card bg-white rounded-2xl shadow-lg overflow-hidden" 
             draggable="true" 
             data-category-id="${category.id}"
             ondragstart="handleCategoryDragStart(event)"
             ondragover="handleCategoryDragOver(event)"
             ondrop="handleCategoryDrop(event)"
             ondragend="handleCategoryDragEnd(event)"
             ontouchstart="handleTouchStart(event, 'category')"
             ontouchmove="handleTouchMove(event)"
             ontouchend="handleTouchEnd(event)">
            <div class="card-content p-5 md:p-6">
                <!-- Header -->
                <div class="flex items-center justify-between mb-4">
                    <div class="flex items-center space-x-3">
                        <div class="w-10 h-10 rounded-xl flex items-center justify-center text-2xl ${colorClass}">
                            ${getIconEmoji(category.icon)}
                        </div>
                        <div>
                            <div class="flex items-center space-x-2 mb-1">
                                <h3 class="font-semibold text-gray-800 text-lg md:text-xl">${category.name}</h3>
                                <span class="min-w-5 h-5 px-1 rounded-full text-xs font-semibold flex items-center justify-center ${badgeColor}">
                                    ${category.taskCount || 0}
                                </span>
                            </div>
                            <p class="text-sm text-gray-500">
                                ${category.taskCount === 0 ? 'Henüz görev yok' : `${category.taskCount} görev`}
                            </p>
                        </div>
                    </div>
                    <div class="flex space-x-1">
                        <button onclick="editCategory('${category.id}')" 
                                class="p-2 text-gray-400 hover:text-blue-500 hover:bg-blue-50 rounded-lg transition-all focus:ring-2 focus:ring-offset-2 focus:ring-indigo-300"
                                aria-label="Kategoriyi düzenle"
                                title="Kategoriyi düzenle">
                            <i data-lucide="edit-2" class="w-4 h-4"></i>
                        </button>
                        <button onclick="deleteCategory('${category.id}')" 
                                class="p-2 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-all focus:ring-2 focus:ring-offset-2 focus:ring-indigo-300"
                                aria-label="Kategoriyi sil"
                                title="Kategoriyi sil">
                            <i data-lucide="trash-2" class="w-4 h-4"></i>
                        </button>
                    </div>
                </div>
                
                <!-- Description -->
                ${category.description ? `<p class="text-sm text-gray-500 mb-4 leading-relaxed">${category.description}</p>` : ''}
                
                <!-- Tasks Section -->
                <div class="tasks-section mb-4">
                    <div class="space-y-2" id="tasks-${category.id}"
                         ondragover="handleTaskDragOver(event)"
                         ondrop="handleTaskDrop(event)">
                        <div class="text-center py-8 text-gray-400">
                            <div class="animate-spin rounded-full h-6 w-6 border-b-2 border-gray-300 mx-auto"></div>
                            <p class="text-xs mt-2">Görevler yükleniyor...</p>
                        </div>
                    </div>
                </div>
                
                <!-- Add Task Button -->
                <button onclick="addTask('${category.id}')" 
                        class="w-full h-11 font-medium shadow-sm bg-gradient-to-r from-indigo-500 to-fuchsia-500 text-white rounded-xl hover:from-indigo-600 hover:to-fuchsia-600 active:scale-[0.98] transition-all flex items-center justify-center space-x-2 focus:ring-2 focus:ring-offset-2 focus:ring-indigo-300">
                    <i data-lucide="plus" class="w-4 h-4"></i>
                    <span>Görev Ekle</span>
                </button>
            </div>
        </div>
        `;
    }).join('');

    // Load tasks for each category
    categories.forEach(category => loadTasksForCategory(category.id));
    
    // Re-initialize icons
    lucide.createIcons();
};

const loadTasksForCategory = async (categoryId) => {
    try {
        const tasks = await loadTasks(categoryId);
        renderTasks(categoryId, tasks.items || tasks);
    } catch (error) {
        document.getElementById(`tasks-${categoryId}`).innerHTML = `
            <p class="text-red-500 text-sm text-center">Görevler yüklenemedi</p>
        `;
    }
};

const renderTasks = (categoryId, tasks) => {
    const tasksContainer = document.getElementById(`tasks-${categoryId}`);
    
    if (tasks.length === 0) {
        tasksContainer.innerHTML = `
            <div class="text-center py-6 flex flex-col items-center justify-center h-24">
                <div class="w-12 h-12 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-3">
                    <i data-lucide="inbox" class="w-6 h-6 text-gray-400"></i>
                </div>
                <p class="text-gray-400 text-sm">Henüz görev yok</p>
            </div>
        `;
        lucide.createIcons();
        return;
    }

    tasksContainer.innerHTML = tasks.map(task => `
        <div class="flex items-center space-x-3 p-3 rounded-xl hover:bg-gray-50 group transition-all border border-transparent hover:border-gray-200 ${task.isDone ? 'opacity-60 hover:opacity-80 order-last' : ''}"
             draggable="true"
             data-task-id="${task.id}"
             ondragstart="handleTaskDragStart(event)"
             ondragend="handleTaskDragEnd(event)"
             ontouchstart="handleTouchStart(event, 'task')"
             ontouchmove="handleTouchMove(event)"
             ontouchend="handleTouchEnd(event)">
            <button onclick="toggleTaskStatus('${task.id}')" 
                    class="flex-shrink-0 focus:ring-2 focus:ring-offset-2 focus:ring-indigo-300 rounded-full">
                <div class="w-5 h-5 rounded-full border-2 ${task.isDone ? 'bg-emerald-500 border-emerald-500' : 'border-gray-300 hover:border-green-400'} flex items-center justify-center transition-all">
                    ${task.isDone ? '<i data-lucide="check" class="w-3 h-3 text-white"></i>' : ''}
                </div>
            </button>
            
            <div class="flex-1 min-w-0">
                <p class="text-sm font-medium ${task.isDone ? 'line-through text-gray-400' : 'text-gray-700'} truncate">
                    ${task.title}
                </p>
                ${task.description ? `<p class="text-xs ${task.isDone ? 'text-gray-400' : 'text-gray-500'} truncate mt-1">${task.description}</p>` : ''}
            </div>
            
            <div class="flex space-x-1 opacity-0 group-hover:opacity-100 transition-all">
                <button onclick="editTask('${task.id}', '${categoryId}')" 
                        class="p-2 text-gray-400 hover:text-blue-500 hover:bg-blue-50 rounded-lg transition-all focus:ring-2 focus:ring-offset-2 focus:ring-indigo-300"
                        aria-label="Görevi düzenle"
                        title="Görevi düzenle">
                    <i data-lucide="edit-2" class="w-3 h-3"></i>
                </button>
                <button onclick="deleteTaskById('${task.id}')" 
                        class="p-2 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-all focus:ring-2 focus:ring-offset-2 focus:ring-indigo-300"
                        aria-label="Görevi sil"
                        title="Görevi sil">
                    <i data-lucide="trash-2" class="w-3 h-3"></i>
                </button>
            </div>
        </div>
    `).join('');
    
    lucide.createIcons();
};

// Event Handlers
const openCategoryModal = (category = null) => {
    isEditMode = !!category;
    currentCategoryId = category?.id || null;
    
    document.getElementById('modalTitle').textContent = isEditMode ? 'Kategori Düzenle' : 'Yeni Kategori';
    document.getElementById('categoryName').value = category?.name || '';
    document.getElementById('categoryDescription').value = category?.description || '';
    document.getElementById('categoryColor').value = category?.color || '#3B82F6';
    document.getElementById('categoryIcon').value = category?.icon || 'folder';
    
    showModal(categoryModal);
};

const editCategory = (categoryId) => {
    const category = categories.find(c => c.id === categoryId);
    if (category) {
        openCategoryModal(category);
    }
};

const deleteCategoryById = async (categoryId) => {
    if (!confirm('Bu kategoriyi silmek istediğinizden emin misiniz?')) return;
    
    try {
        const result = await deleteCategory(categoryId);
        console.log('Delete result:', result); // Debug log
        showToast('Kategori başarıyla silindi!', 'success');
        await loadCategories();
    } catch (error) {
        console.error('Delete category failed:', error);
        // Error toast already shown in apiCall, but show backup message
        if (!error.message.includes('Hata:')) {
            showToast('Kategori silinemedi', 'error');
        }
    }
};

const addTask = (categoryId) => {
    currentCategoryId = categoryId;
    currentTaskId = null;
    isEditMode = false;
    
    // Find category name to display
    const category = categories.find(c => c.id === categoryId);
    const categoryName = category ? category.name : 'Bilinmeyen Kategori';
    
    document.getElementById('taskModalTitle').textContent = 'Yeni Görev';
    document.getElementById('taskModalCategory').textContent = `Kategori: ${categoryName}`;
    document.getElementById('taskTitle').value = '';
    document.getElementById('taskDescription').value = '';
    
    showModal(taskModal);
};

const editTask = async (taskId, categoryId) => {
    try {
        const task = await apiCall(`/Tasks/${taskId}`);
        currentTaskId = taskId;
        currentCategoryId = categoryId;
        isEditMode = true;
        
        // Find category name to display
        const category = categories.find(c => c.id === categoryId);
        const categoryName = category ? category.name : 'Bilinmeyen Kategori';
        
        document.getElementById('taskModalTitle').textContent = 'Görevi Düzenle';
        document.getElementById('taskModalCategory').textContent = `Kategori: ${categoryName}`;
        document.getElementById('taskTitle').value = task.title;
        document.getElementById('taskDescription').value = task.description || '';
        
        showModal(taskModal);
    } catch (error) {
        showToast('Görev bilgileri alınamadı', 'error');
    }
};

const toggleTaskStatus = async (taskId) => {
    try {
        await toggleTask(taskId);
        // Reload tasks for all categories
        categories.forEach(category => loadTasksForCategory(category.id));
        showToast('Görev durumu güncellendi');
    } catch (error) {
        showToast('Görev durumu güncellenemedi', 'error');
    }
};

const deleteTaskById = async (taskId) => {
    if (!confirm('Bu görevi silmek istediğinizden emin misiniz?')) return;
    
    try {
        await deleteTask(taskId);
        showToast('Görev silindi');
        // Reload tasks for all categories
        categories.forEach(category => loadTasksForCategory(category.id));
    } catch (error) {
        showToast('Görev silinemedi', 'error');
    }
};

// Form Handlers
categoryForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const formData = {
        name: document.getElementById('categoryName').value.trim(),
        description: document.getElementById('categoryDescription').value.trim(),
        color: document.getElementById('categoryColor').value,
        icon: document.getElementById('categoryIcon').value,
        isActive: true
    };
    
    if (!formData.name) {
        showToast('Kategori adı gerekli', 'error');
        return;
    }
    
    try {
        if (isEditMode && currentCategoryId) {
            await updateCategory(currentCategoryId, { ...formData, id: currentCategoryId });
            showToast('Kategori güncellendi');
        } else {
            await createCategory(formData);
            showToast('Kategori oluşturuldu');
        }
        
        hideModal(categoryModal);
        loadCategories();
    } catch (error) {
        showToast(isEditMode ? 'Kategori güncellenemedi' : 'Kategori oluşturulamadı', 'error');
    }
});

taskForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const formData = {
        title: document.getElementById('taskTitle').value.trim(),
        description: document.getElementById('taskDescription').value.trim(),
        categoryId: currentCategoryId
    };
    
    if (!formData.title) {
        showToast('Görev adı gerekli', 'error');
        return;
    }
    
    if (!formData.categoryId) {
        showToast('Kategori seçilmedi', 'error');
        return;
    }
    
    try {
        if (isEditMode && currentTaskId) {
            await updateTask(currentTaskId, { ...formData, id: currentTaskId, isDone: false });
            showToast('Görev güncellendi', 'success');
        } else {
            await createTask(formData);
            showToast('Görev oluşturuldu', 'success');
        }
        
        hideModal(taskModal);
        // Reload tasks for the specific category only
        await loadTasksForCategory(currentCategoryId);
    } catch (error) {
        showToast(isEditMode ? 'Görev güncellenemedi' : 'Görev oluşturulamadı', 'error');
    }
});

// Event Listeners
document.getElementById('addCategoryBtn').addEventListener('click', () => openCategoryModal());
document.getElementById('addFirstCategoryBtn').addEventListener('click', () => openCategoryModal());
document.getElementById('closeModal').addEventListener('click', () => hideModal(categoryModal));
document.getElementById('cancelBtn').addEventListener('click', () => hideModal(categoryModal));
document.getElementById('closeTaskModal').addEventListener('click', () => hideModal(taskModal));
document.getElementById('cancelTaskBtn').addEventListener('click', () => hideModal(taskModal));

// Close modals on outside click
categoryModal.addEventListener('click', (e) => {
    if (e.target === categoryModal) hideModal(categoryModal);
});
taskModal.addEventListener('click', (e) => {
    if (e.target === taskModal) hideModal(taskModal);
});

// Keyboard shortcuts
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
        hideModal(categoryModal);
        hideModal(taskModal);
    }
});

// Drag & Drop Handlers for Categories
const handleCategoryDragStart = (e) => {
    draggedCategoryId = e.target.closest('.card').dataset.categoryId;
    e.target.style.opacity = '0.5';
    e.dataTransfer.effectAllowed = 'move';
};

const handleCategoryDragOver = (e) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
    
    const card = e.target.closest('.card');
    if (card && card.dataset.categoryId !== draggedCategoryId) {
        card.style.borderTop = '3px solid #3B82F6';
    }
};

const handleCategoryDrop = async (e) => {
    e.preventDefault();
    
    const targetCard = e.target.closest('.card');
    const targetCategoryId = targetCard.dataset.categoryId;
    
    if (draggedCategoryId && targetCategoryId && draggedCategoryId !== targetCategoryId) {
        try {
            // Reorder categories logic
            await reorderCategories(draggedCategoryId, targetCategoryId);
            showToast('Kategori sıralaması güncellendi', 'success');
        } catch (error) {
            showToast('Sıralama güncellenemedi', 'error');
        }
    }
    
    // Reset styles
    document.querySelectorAll('.card').forEach(card => {
        card.style.borderTop = '';
        card.style.opacity = '';
    });
};

const handleCategoryDragEnd = (e) => {
    draggedCategoryId = null;
    e.target.style.opacity = '';
    
    // Reset all border styles
    document.querySelectorAll('.card').forEach(card => {
        card.style.borderTop = '';
    });
};

// Drag & Drop Handlers for Tasks
const handleTaskDragStart = (e) => {
    draggedTaskId = e.target.closest('[data-task-id]').dataset.taskId;
    e.target.style.opacity = '0.5';
    e.dataTransfer.effectAllowed = 'move';
};

const handleTaskDragOver = (e) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
    
    const tasksContainer = e.target.closest('[id^="tasks-"]');
    if (tasksContainer) {
        tasksContainer.style.backgroundColor = '#EFF6FF';
    }
};

const handleTaskDrop = async (e) => {
    e.preventDefault();
    
    const tasksContainer = e.target.closest('[id^="tasks-"]');
    if (tasksContainer && draggedTaskId) {
        const newCategoryId = tasksContainer.id.replace('tasks-', '');
        
        try {
            // Move task to new category
            await moveTaskToCategory(draggedTaskId, newCategoryId);
            showToast('Görev taşındı', 'success');
        } catch (error) {
            showToast('Görev taşınamadı', 'error');
        }
    }
    
    // Reset styles
    document.querySelectorAll('[id^="tasks-"]').forEach(container => {
        container.style.backgroundColor = '';
    });
};

const handleTaskDragEnd = (e) => {
    draggedTaskId = null;
    e.target.style.opacity = '';
    
    // Reset all background styles
    document.querySelectorAll('[id^="tasks-"]').forEach(container => {
        container.style.backgroundColor = '';
    });
};

// Helper functions for drag & drop operations
const reorderCategories = async (draggedId, targetId) => {
    const draggedIndex = categories.findIndex(c => c.id === draggedId);
    const targetIndex = categories.findIndex(c => c.id === targetId);
    
    if (draggedIndex === -1 || targetIndex === -1) return;
    
    // Create new order
    const newCategories = [...categories];
    const [draggedCategory] = newCategories.splice(draggedIndex, 1);
    newCategories.splice(targetIndex, 0, draggedCategory);
    
    // Update sort orders
    const reorderData = newCategories.map((category, index) => ({
        id: category.id,
        sortOrder: index + 1
    }));
    
    // Send to API
    await apiCall('/Categories/reorder', {
        method: 'PUT',
        body: JSON.stringify({ categories: reorderData })
    });
    
    // Reload categories to reflect new order
    await loadCategories();
};

const moveTaskToCategory = async (taskId, newCategoryId) => {
    // Get current task data
    const task = await apiCall(`/Tasks/${taskId}`);
    
    // Update task with new category
    const updateData = {
        ...task,
        categoryId: newCategoryId
    };
    
    await apiCall(`/Tasks/${taskId}`, {
        method: 'PUT',
        body: JSON.stringify(updateData)
    });
    
    // Reload all categories to reflect changes
    await loadCategories();
};

// Page Navigation
const showTasksPage = () => {
    currentPage = 'tasks';
    tasksPage.classList.remove('hidden');
    habitsPage.classList.add('hidden');
    
    // Update tab styles
    tasksTab.className = 'px-4 py-2 rounded-md text-sm font-medium transition-all bg-white text-gray-900 shadow-sm';
    habitsTab.className = 'px-4 py-2 rounded-md text-sm font-medium transition-all text-gray-600 hover:text-gray-900';
    
    // Update FAB
    document.getElementById('addCategoryBtn').style.display = 'flex';
};

const showHabitsPage = () => {
    currentPage = 'habits';
    habitsPage.classList.remove('hidden');
    tasksPage.classList.add('hidden');
    
    // Update tab styles
    habitsTab.className = 'px-4 py-2 rounded-md text-sm font-medium transition-all bg-white text-gray-900 shadow-sm';
    tasksTab.className = 'px-4 py-2 rounded-md text-sm font-medium transition-all text-gray-600 hover:text-gray-900';
    
    // Update FAB
    document.getElementById('addCategoryBtn').style.display = 'none';
    
    // Load habits
    loadHabits();
};

// Habits API Functions
const loadHabits = async () => {
    try {
        const weekStart = getWeekStart(currentWeekStart);
        const weekStartStr = weekStart.toISOString().split('T')[0]; // YYYY-MM-DD format
        habits = await apiCall(`/Habits?weekStartDate=${weekStartStr}`);
        renderHabitsTable();
        updateWeekDisplay();
    } catch (error) {
        console.error('Load habits failed:', error);
        showToast('Alışkanlıklar yüklenemedi', 'error');
    }
};

    const renderHabitsTable = () => {
        if (habits.length === 0) {
            habitsTable.innerHTML = `
                <div class="text-center py-12">
                    <div class="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <i data-lucide="target" class="w-8 h-8 text-gray-400"></i>
                    </div>
                    <h3 class="text-lg font-medium text-gray-800 mb-2">Henüz alışkanlık yok</h3>
                    <p class="text-gray-500 mb-4">İlk alışkanlığınızı ekleyerek başlayın</p>
                    <button onclick="addHabit()" class="px-4 py-2 bg-gradient-to-r from-blue-500 to-purple-600 text-white rounded-lg hover:shadow-lg transition-all">
                        <i data-lucide="plus" class="w-4 h-4 inline mr-2"></i>
                        Alışkanlık Ekle
                    </button>
                </div>
            `;
            lucide.createIcons();
            return;
        }

        // Generate week days
        const weekStart = getWeekStart(currentWeekStart);
        const weekDays = Array.from({ length: 7 }, (_, i) => {
            const date = new Date(weekStart);
            date.setDate(date.getDate() + i);
            return date;
        });

        const dayNames = ['Paz', 'Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt'];
        const todayIndex = weekDays.findIndex(day => isToday(day));

        // Desktop table view
        const desktopView = `
            <div class="hidden sm:block">
                <table class="habits-table w-full">
                    <thead class="bg-gray-50">
                        <tr>
                            <th class="habit-cell px-4 py-3 text-left text-sm font-medium text-gray-700">Alışkanlık</th>
                            ${weekDays.map((day, index) => `
                                <th class="day-cell day-header px-2 py-3 text-center text-sm font-medium text-gray-700 ${index === todayIndex ? 'bg-indigo-50 font-semibold' : ''}">
                                    <div class="flex flex-col items-center">
                                        <span class="day-name text-xs text-gray-500">${dayNames[index]}</span>
                                        <span class="day-number text-sm font-semibold">${day.getDate()}</span>
                                    </div>
                                </th>
                            `).join('')}
                        </tr>
                    </thead>
                    <tbody id="habitsTableBody">
                        ${habits.map((habit, index) => {
                            const habitIcon = getHabitIcon(habit.name, habit.icon);
                            return `
                                <tr class="hover:bg-gray-50 habit-row ${getHabitRowColorClass(habit.color)} ${index < habits.length - 1 ? 'border-b border-gray-200' : ''}" 
                                    data-habit-id="${habit.id}" 
                                    draggable="true" 
                                    ondragstart="handleHabitDragStart(event, '${habit.id}')" 
                                    ondragover="handleHabitDragOver(event)" 
                                    ondrop="handleHabitDrop(event, '${habit.id}')" 
                                    ondragend="handleHabitDragEnd(event)">
                                    <td class="habit-cell px-4 py-4">
                                        <div class="flex items-center space-x-3">
                                            <div class="w-8 h-8 rounded-lg flex items-center justify-center text-gray-400 cursor-move hover:bg-gray-100 transition-colors" title="Sürükle">
                                                <i data-lucide="grip-vertical" class="w-4 h-4"></i>
                                            </div>
                                            <div class="w-10 h-10 ${getHabitColorClass(habit.color)} rounded-lg p-1 flex items-center justify-center text-xl flex-shrink-0 cursor-pointer hover:opacity-80 transition-opacity" 
                                                 onclick="editHabit('${habit.id}')" 
                                                 title="Alışkanlığı düzenle">
                                                ${getIconEmoji(habitIcon)}
                                            </div>
                                            <div class="min-w-0 flex-1 cursor-pointer" onclick="editHabit('${habit.id}')" title="Alışkanlığı düzenle">
                                                <p class="habit-name font-semibold text-gray-900 truncate">${habit.name}</p>
                                                ${habit.description ? `<p class="habit-description text-xs text-gray-500 truncate">${habit.description}</p>` : ''}
                                                ${habit.type === 1 ? `<p class="habit-description text-xs text-gray-400">Hedef: ${habit.targetValue || 0} ${habit.unit || ''}</p>` : ''}
                                            </div>
                                            <div class="flex space-x-1">
                                                <button onclick="deleteHabit('${habit.id}')" 
                                                        class="p-2 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-all focus:ring-2 focus:ring-offset-2 focus:ring-indigo-300"
                                                        aria-label="Alışkanlığı sil"
                                                        title="Alışkanlığı sil">
                                                    <i data-lucide="trash-2" class="w-4 h-4"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </td>
                                    ${weekDays.map((day, dayIndex) => {
                                        const dateStr = day.toISOString().split('T')[0];
                                        const entry = habit.weeklyEntries?.find(e => e.date === dateStr);
                                        const isTodayColumn = dayIndex === todayIndex;
                                        
                                        return `
                                            <td class="day-cell px-2 py-4 text-center ${isTodayColumn ? 'bg-indigo-50 hover:bg-indigo-100' : ''}">
                                                <div class="flex justify-center items-center h-full">
                                                    ${habit.type === 0 ? 
                                                        // Boolean habit
                                                        `<button onclick="toggleHabitEntry('${habit.id}', '${dateStr}')" 
                                                                class="habit-checkbox w-8 h-8 rounded-full border-2 ${entry?.isCompleted ? 'bg-emerald-500 border-emerald-500 text-white' : 'bg-gray-100 border-gray-300 text-gray-400 hover:border-green-400'} 
                                                                flex items-center justify-center transition-all hover:scale-110 focus:ring-2 focus:ring-indigo-300">
                                                            ${entry?.isCompleted ? '<i data-lucide="check" class="w-4 h-4"></i>' : ''}
                                                        </button>` :
                                                        // Numeric habit
                                                        `<div class="relative">
                                                            <input type="number" 
                                                                    value="${entry?.value || ''}" 
                                                                    placeholder="–"
                                                                    onchange="updateHabitValue('${habit.id}', '${dateStr}', this.value)"
                                                                    class="w-16 h-8 text-center text-sm border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-300 focus:border-transparent ${(entry?.value || 0) >= (habit.targetValue || 1) ? 'bg-green-50 border-green-300' : ''}"
                                                                    min="0" max="999">
                                                            ${(entry?.value || 0) >= (habit.targetValue || 1) ? '<div class="absolute -top-1 -right-1 w-3 h-3 bg-emerald-500 rounded-full"></div>' : ''}
                                                        </div>`
                                                    }
                                                </div>
                                            </td>
                                        `;
                                    }).join('')}
                                </tr>
                            `;
                        }).join('')}
                    </tbody>
                </table>
            </div>
        `;

        // Mobile card view
        const mobileView = `
            <div class="sm:hidden space-y-4">
                ${habits.map((habit, index) => {
                    const habitIcon = getHabitIcon(habit.name, habit.icon);
                    return `
                        <div class="bg-white shadow-sm rounded-lg p-4 ${getHabitRowColorClass(habit.color)}">
                            <div class="flex items-center space-x-3 mb-4">
                                <div class="w-10 h-10 ${getHabitColorClass(habit.color)} rounded-lg p-1 flex items-center justify-center text-xl cursor-pointer hover:opacity-80 transition-opacity" 
                                     onclick="editHabit('${habit.id}')" 
                                     title="Alışkanlığı düzenle">
                                    ${getIconEmoji(habitIcon)}
                                </div>
                                <div class="flex-1 cursor-pointer" onclick="editHabit('${habit.id}')" title="Alışkanlığı düzenle">
                                    <h3 class="font-semibold text-gray-900">${habit.name}</h3>
                                    ${habit.description ? `<p class="text-xs text-gray-500">${habit.description}</p>` : ''}
                                    ${habit.type === 1 ? `<p class="text-xs text-gray-400">Hedef: ${habit.targetValue || 0} ${habit.unit || ''}</p>` : ''}
                                </div>
                                <button onclick="deleteHabit('${habit.id}')" 
                                        class="p-2 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-all focus:ring-2 focus:ring-offset-2 focus:ring-indigo-300"
                                        aria-label="Alışkanlığı sil"
                                        title="Alışkanlığı sil">
                                    <i data-lucide="trash-2" class="w-4 h-4"></i>
                                </button>
                            </div>
                            <div class="flex justify-between items-center">
                                ${weekDays.map((day, dayIndex) => {
                                    const dateStr = day.toISOString().split('T')[0];
                                    const entry = habit.weeklyEntries?.find(e => e.date === dateStr);
                                    const isTodayColumn = dayIndex === todayIndex;
                                    
                                    return `
                                        <div class="flex flex-col items-center ${isTodayColumn ? 'bg-indigo-50 rounded-lg p-2' : ''}">
                                            <span class="text-xs text-gray-500 mb-1">${dayNames[dayIndex]}</span>
                                            <span class="text-sm font-semibold mb-2">${day.getDate()}</span>
                                            ${habit.type === 0 ? 
                                                `<button onclick="toggleHabitEntry('${habit.id}', '${dateStr}')" 
                                                        class="w-6 h-6 rounded-full border-2 ${entry?.isCompleted ? 'bg-emerald-500 border-emerald-500 text-white' : 'bg-gray-100 border-gray-300 text-gray-400'} 
                                                        flex items-center justify-center transition-all focus:ring-2 focus:ring-indigo-300">
                                                    ${entry?.isCompleted ? '<i data-lucide="check" class="w-3 h-3"></i>' : ''}
                                                </button>` :
                                                `<input type="number" 
                                                        value="${entry?.value || ''}" 
                                                        placeholder="–"
                                                        onchange="updateHabitValue('${habit.id}', '${dateStr}', this.value)"
                                                        class="w-12 h-6 text-center text-xs border border-gray-300 rounded focus:ring-2 focus:ring-indigo-300 ${(entry?.value || 0) >= (habit.targetValue || 1) ? 'bg-green-50 border-green-300' : ''}"
                                                        min="0" max="999">`
                                            }
                                        </div>
                                    `;
                                }).join('')}
                            </div>
                        </div>
                    `;
                }).join('')}
            </div>
        `;

        habitsTable.innerHTML = desktopView + mobileView;
        
        lucide.createIcons();
    };

// Utility functions for date handling
const getWeekStart = (date) => {
    const d = new Date(date);
    const day = d.getDay();
    const diff = d.getDate() - day; // Sunday = 0
    return new Date(d.setDate(diff));
};

const updateWeekDisplay = () => {
    const weekStart = getWeekStart(currentWeekStart);
    const weekEnd = new Date(weekStart);
    weekEnd.setDate(weekEnd.getDate() + 6);
    
    const options = { day: 'numeric', month: 'long' };
    const startStr = weekStart.toLocaleDateString('tr-TR', options);
    const endStr = weekEnd.toLocaleDateString('tr-TR', options);
    
    document.getElementById('weekRange').textContent = `${startStr} - ${endStr}`;
};

// Habit interaction functions
const toggleHabitEntry = async (habitId, dateStr) => {
    try {
        const habit = habits.find(h => h.id === habitId);
        const entry = habit?.weeklyEntries?.find(e => e.date === dateStr);
        
        await apiCall(`/Habits/${habitId}/entries`, {
            method: 'PUT',
            body: JSON.stringify({
                date: dateStr,
                isCompleted: !entry?.isCompleted,
                value: null,
                notes: null
            })
        });
        
        await loadHabits(); // Reload to reflect changes
        showToast('Alışkanlık güncellendi', 'success');
    } catch (error) {
        showToast('Güncelleme başarısız', 'error');
    }
};

const updateHabitValue = async (habitId, dateStr, value) => {
    try {
        const numValue = parseInt(value) || 0;
        
        await apiCall(`/Habits/${habitId}/entries`, {
            method: 'PUT',
            body: JSON.stringify({
                date: dateStr,
                isCompleted: numValue > 0,
                value: numValue,
                notes: null
            })
        });
        
        showToast('Değer güncellendi', 'success');
    } catch (error) {
        showToast('Güncelleme başarısız', 'error');
    }
};

    // Initialize the app
    document.addEventListener('DOMContentLoaded', () => {
        loadCategories();
        
        // Tab event listeners
        tasksTab.addEventListener('click', showTasksPage);
        habitsTab.addEventListener('click', showHabitsPage);
        
        // Week navigation
        document.getElementById('prevWeek').addEventListener('click', () => {
            currentWeekStart.setDate(currentWeekStart.getDate() - 7);
            loadHabits();
        });
        
        document.getElementById('nextWeek').addEventListener('click', () => {
            currentWeekStart.setDate(currentWeekStart.getDate() + 7);
            loadHabits();
        });
        
        // Add habit button
        document.getElementById('addHabitBtn').addEventListener('click', addHabit);
        
        // Habit form submission
        document.getElementById('habitForm').addEventListener('submit', submitHabitForm);
    });

// Habit drag & drop functions
let draggedHabitId = null;

const handleHabitDragStart = (event, habitId) => {
    draggedHabitId = habitId;
    event.target.style.opacity = '0.5';
    event.dataTransfer.effectAllowed = 'move';
    event.dataTransfer.setData('text/html', event.target.outerHTML);
};

const handleHabitDragOver = (event) => {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';
    
    const targetRow = event.target.closest('.habit-row');
    if (targetRow && targetRow.dataset.habitId !== draggedHabitId) {
        targetRow.classList.add('drag-over');
    }
};

const handleHabitDrop = async (event, targetHabitId) => {
    event.preventDefault();
    
    if (draggedHabitId === targetHabitId) {
        return;
    }
    
    // Remove drag-over class from all rows
    document.querySelectorAll('.habit-row').forEach(row => {
        row.classList.remove('drag-over');
    });
    
    // Find the dragged and target habits
    const draggedIndex = habits.findIndex(h => h.id === draggedHabitId);
    const targetIndex = habits.findIndex(h => h.id === targetHabitId);
    
    if (draggedIndex === -1 || targetIndex === -1) {
        return;
    }
    
    // Reorder habits array
    const draggedHabit = habits.splice(draggedIndex, 1)[0];
    habits.splice(targetIndex, 0, draggedHabit);
    
    // Update sort orders
    const reorderData = habits.map((habit, index) => ({
        id: habit.id,
        sortOrder: index + 1
    }));
    
    try {
        await apiCall('/Habits/reorder', {
            method: 'PUT',
            body: JSON.stringify({ habits: reorderData })
        });
        
        // Re-render the table
        renderHabitsTable();
        showToast('Alışkanlıklar yeniden sıralandı', 'success');
    } catch (error) {
        console.error('Reorder habits failed:', error);
        showToast('Sıralama başarısız', 'error');
        // Reload habits to restore original order
        await loadHabits();
    }
};

const handleHabitDragEnd = (event) => {
    event.target.style.opacity = '';
    draggedHabitId = null;
    
    // Remove drag-over class from all rows
    document.querySelectorAll('.habit-row').forEach(row => {
        row.classList.remove('drag-over');
    });
};

// Add habit function
const addHabit = () => {
    document.getElementById('habitModal').classList.remove('hidden');
    document.body.style.overflow = 'hidden';
    document.getElementById('habitForm').reset();
    document.getElementById('numericFields').classList.add('hidden');
    selectedHabitColor = '#3B82F6';
    
    // Reset color selection visual feedback
    document.querySelectorAll('#habitModal button[data-color]').forEach(btn => {
        btn.classList.remove('border-blue-500');
        btn.classList.add('border-transparent');
    });
    // Set default blue color as selected
    const defaultColorBtn = document.querySelector('#habitModal button[data-color="#3B82F6"]');
    if (defaultColorBtn) {
        defaultColorBtn.classList.remove('border-transparent');
        defaultColorBtn.classList.add('border-blue-500');
    }
    
    // Add event listeners for color buttons
    document.querySelectorAll('#habitModal button[data-color]').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const color = e.target.getAttribute('data-color');
            selectHabitColor(color, e.target);
        });
    });
    
    // Set modal title and form action
    document.querySelector('#habitModal h3').textContent = 'Yeni Alışkanlık';
    document.getElementById('habitForm').dataset.action = 'create';
    document.getElementById('habitForm').dataset.habitId = '';
    document.getElementById('habitSubmitButton').textContent = 'Ekle';
};

// Edit habit function
const editHabit = (habitId) => {
    const habit = habits.find(h => h.id === habitId);
    if (!habit) return;
    
    document.getElementById('habitModal').classList.remove('hidden');
    document.body.style.overflow = 'hidden';
    
    // Fill form with habit data
    document.getElementById('habitName').value = habit.name;
    document.getElementById('habitDescription').value = habit.description || '';
    document.getElementById('habitType').value = habit.type.toString();
    document.getElementById('habitIcon').value = habit.icon;
    document.getElementById('habitTargetValue').value = habit.targetValue || '';
    document.getElementById('habitUnit').value = habit.unit || '';
    
    // Set color
    selectedHabitColor = habit.color || '#3B82F6';
    
    // Reset color selection visual feedback
    document.querySelectorAll('#habitModal button[data-color]').forEach(btn => {
        btn.classList.remove('border-blue-500');
        btn.classList.add('border-transparent');
    });
    // Set selected color
    const selectedColorBtn = document.querySelector(`#habitModal button[data-color="${selectedHabitColor}"]`);
    if (selectedColorBtn) {
        selectedColorBtn.classList.remove('border-transparent');
        selectedColorBtn.classList.add('border-blue-500');
    }
    
    // Show/hide numeric fields based on type
    const numericFields = document.getElementById('numericFields');
    if (habit.type === 1) {
        numericFields.classList.remove('hidden');
    } else {
        numericFields.classList.add('hidden');
    }
    
    // Add event listeners for color buttons
    document.querySelectorAll('#habitModal button[data-color]').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const color = e.target.getAttribute('data-color');
            selectHabitColor(color, e.target);
        });
    });
    
    // Set modal title and form action
    document.querySelector('#habitModal h3').textContent = 'Alışkanlığı Düzenle';
    document.getElementById('habitForm').dataset.action = 'edit';
    document.getElementById('habitForm').dataset.habitId = habitId;
    document.getElementById('habitSubmitButton').textContent = 'Güncelle';
};

// Close habit modal
const closeHabitModal = () => {
    document.getElementById('habitModal').classList.add('hidden');
    document.body.style.overflow = 'auto';
    document.getElementById('habitForm').reset();
    document.getElementById('numericFields').classList.add('hidden');
};

// Toggle habit type fields
const toggleHabitTypeFields = () => {
    const habitType = document.getElementById('habitType').value;
    const numericFields = document.getElementById('numericFields');
    
    if (habitType === '1') {
        numericFields.classList.remove('hidden');
    } else {
        numericFields.classList.add('hidden');
    }
};

// Select habit color
let selectedHabitColor = '#3B82F6';
const selectHabitColor = (color, element) => {
    console.log('selectHabitColor called with:', color, element);
    selectedHabitColor = color;
    // Update visual feedback
    document.querySelectorAll('#habitModal button[data-color]').forEach(btn => {
        btn.classList.remove('border-blue-500');
        btn.classList.add('border-transparent');
    });
    element.classList.remove('border-transparent');
    element.classList.add('border-blue-500');
    console.log('Color selected:', selectedHabitColor);
};

// Delete habit function
const deleteHabit = async (habitId) => {
    if (!confirm('Bu alışkanlığı silmek istediğinizden emin misiniz?')) return;
    
    try {
        await apiCall(`/Habits/${habitId}`, {
            method: 'DELETE'
        });
        
        showToast('Alışkanlık silindi', 'success');
        await loadHabits();
    } catch (error) {
        console.error('Delete habit failed:', error);
        showToast('Alışkanlık silinemedi', 'error');
    }
};

// Submit habit form
const submitHabitForm = async (e) => {
    e.preventDefault();
    
    const form = document.getElementById('habitForm');
    const action = form.dataset.action;
    const habitId = form.dataset.habitId;
    
    const formData = {
        name: document.getElementById('habitName').value,
        description: document.getElementById('habitDescription').value,
        type: parseInt(document.getElementById('habitType').value),
        color: selectedHabitColor,
        icon: document.getElementById('habitIcon').value,
        targetValue: document.getElementById('habitTargetValue').value ? parseInt(document.getElementById('habitTargetValue').value) : null,
        unit: document.getElementById('habitUnit').value || null,
        isActive: true
    };
    
    // Add sortOrder only for create action
    if (action === 'create') {
        formData.sortOrder = habits.length + 1;
    }
    
    // Check for type change in edit mode
    if (action === 'edit') {
        const originalHabit = habits.find(h => h.id === habitId);
        if (originalHabit && originalHabit.type !== formData.type) {
            const typeChangeMessage = formData.type === 0 
                ? 'Alışkanlık türünü "Sadece yapıldı/yapılmadı" olarak değiştiriyorsunuz. Mevcut sayısal veriler silinecek.'
                : 'Alışkanlık türünü "Miktar ile takip" olarak değiştiriyorsunuz. Mevcut veriler silinecek.';
            
            if (!confirm(typeChangeMessage + '\n\nDevam etmek istiyor musunuz?')) {
                return;
            }
        }
    }
    
    try {
        if (action === 'create') {
            await apiCall('/Habits', {
                method: 'POST',
                body: JSON.stringify(formData)
            });
            showToast('Alışkanlık başarıyla eklendi!', 'success');
        } else if (action === 'edit') {
            const response = await apiCall(`/Habits/${habitId}`, {
                method: 'PUT',
                body: JSON.stringify(formData)
            });
            
            // Check if type was changed based on response message
            const successMessage = response?.message?.includes('type change') 
                ? 'Alışkanlık güncellendi! Mevcut veriler tür değişikliği nedeniyle temizlendi.'
                : 'Alışkanlık başarıyla güncellendi!';
            
            showToast(successMessage, 'success');
        }
        
        closeHabitModal();
        await loadHabits();
    } catch (error) {
        console.error('Habit operation failed:', error);
        showToast(action === 'create' ? 'Alışkanlık eklenemedi' : 'Alışkanlık güncellenemedi', 'error');
    }
};

// Make functions global for onclick handlers
window.editCategory = editCategory;
window.deleteCategory = deleteCategoryById;
window.addTask = addTask;
window.editTask = editTask;
window.toggleTaskStatus = toggleTaskStatus;
window.deleteTaskById = deleteTaskById;

// Make habit functions global
window.editHabit = editHabit;
window.toggleHabitEntry = toggleHabitEntry;
window.updateHabitValue = updateHabitValue;
window.addHabit = addHabit;
window.deleteHabit = deleteHabit;
window.closeHabitModal = closeHabitModal;
window.toggleHabitTypeFields = toggleHabitTypeFields;
window.selectHabitColor = selectHabitColor;
window.handleHabitDragStart = handleHabitDragStart;
window.handleHabitDragOver = handleHabitDragOver;
window.handleHabitDrop = handleHabitDrop;
window.handleHabitDragEnd = handleHabitDragEnd;

// Touch Event Handlers for Mobile
const handleTouchStart = (e, type) => {
    const touch = e.touches[0];
    touchStartPos = { x: touch.clientX, y: touch.clientY };
    touchDragType = type;
    
    if (type === 'category') {
        draggedElement = e.target.closest('.card');
        draggedCategoryId = draggedElement.dataset.categoryId;
    } else if (type === 'task') {
        draggedElement = e.target.closest('[data-task-id]');
        draggedTaskId = draggedElement.dataset.taskId;
    }
    
    // Prevent default to avoid scrolling
    e.preventDefault();
};

const handleTouchMove = (e) => {
    if (!draggedElement) return;
    
    const touch = e.touches[0];
    const deltaX = touch.clientX - touchStartPos.x;
    const deltaY = touch.clientY - touchStartPos.y;
    const distance = Math.sqrt(deltaX * deltaX + deltaY * deltaY);
    
    // Start dragging if moved more than 10px
    if (distance > 10 && !isDragging) {
        isDragging = true;
        draggedElement.style.opacity = '0.7';
        draggedElement.style.transform = 'scale(0.95)';
        draggedElement.style.zIndex = '1000';
        
        // Add visual feedback class
        document.body.classList.add('is-dragging');
    }
    
    if (isDragging) {
        // Move element with finger
        draggedElement.style.position = 'fixed';
        draggedElement.style.left = (touch.clientX - 100) + 'px';
        draggedElement.style.top = (touch.clientY - 50) + 'px';
        draggedElement.style.pointerEvents = 'none';
        
        // Find element under finger
        const elementBelow = document.elementFromPoint(touch.clientX, touch.clientY);
        
        if (touchDragType === 'category') {
            // Highlight drop zones for categories
            document.querySelectorAll('.card').forEach(card => {
                card.classList.remove('drag-over');
            });
            
            const targetCard = elementBelow?.closest('.card');
            if (targetCard && targetCard !== draggedElement) {
                targetCard.classList.add('drag-over');
            }
        } else if (touchDragType === 'task') {
            // Highlight drop zones for tasks
            document.querySelectorAll('[id^="tasks-"]').forEach(container => {
                container.classList.remove('drop-zone-active');
            });
            
            const targetContainer = elementBelow?.closest('[id^="tasks-"]');
            if (targetContainer) {
                targetContainer.classList.add('drop-zone-active');
            }
        }
    }
    
    e.preventDefault();
};

const handleTouchEnd = async (e) => {
    if (!isDragging || !draggedElement) {
        resetTouchDrag();
        return;
    }
    
    const touch = e.changedTouches[0];
    const elementBelow = document.elementFromPoint(touch.clientX, touch.clientY);
    
    try {
        if (touchDragType === 'category') {
            const targetCard = elementBelow?.closest('.card');
            const targetCategoryId = targetCard?.dataset.categoryId;
            
            if (draggedCategoryId && targetCategoryId && draggedCategoryId !== targetCategoryId) {
                await reorderCategories(draggedCategoryId, targetCategoryId);
                showToast('Kategori sıralaması güncellendi', 'success');
            }
        } else if (touchDragType === 'task') {
            const targetContainer = elementBelow?.closest('[id^="tasks-"]');
            if (targetContainer && draggedTaskId) {
                const newCategoryId = targetContainer.id.replace('tasks-', '');
                await moveTaskToCategory(draggedTaskId, newCategoryId);
                showToast('Görev taşındı', 'success');
            }
        }
    } catch (error) {
        showToast('İşlem tamamlanamadı', 'error');
    }
    
    resetTouchDrag();
};

const resetTouchDrag = () => {
    if (draggedElement) {
        draggedElement.style.opacity = '';
        draggedElement.style.transform = '';
        draggedElement.style.position = '';
        draggedElement.style.left = '';
        draggedElement.style.top = '';
        draggedElement.style.zIndex = '';
        draggedElement.style.pointerEvents = '';
    }
    
    // Remove all visual feedback
    document.querySelectorAll('.card').forEach(card => {
        card.classList.remove('drag-over');
    });
    document.querySelectorAll('[id^="tasks-"]').forEach(container => {
        container.classList.remove('drop-zone-active');
    });
    document.body.classList.remove('is-dragging');
    
    // Reset state
    isDragging = false;
    draggedElement = null;
    draggedCategoryId = null;
    draggedTaskId = null;
    touchDragType = null;
};

// Make drag & drop functions global
window.handleCategoryDragStart = handleCategoryDragStart;
window.handleCategoryDragOver = handleCategoryDragOver;
window.handleCategoryDrop = handleCategoryDrop;
window.handleCategoryDragEnd = handleCategoryDragEnd;
window.handleTaskDragStart = handleTaskDragStart;
window.handleTaskDragOver = handleTaskDragOver;
window.handleTaskDrop = handleTaskDrop;
window.handleTaskDragEnd = handleTaskDragEnd;

// Make touch functions global
window.handleTouchStart = handleTouchStart;
window.handleTouchMove = handleTouchMove;
window.handleTouchEnd = handleTouchEnd;

// Habit icon selection function
function selectHabitIcon(iconValue) {
    document.getElementById('habitIcon').value = iconValue;
    
    // Update visual selection
    document.querySelectorAll('[onclick^="selectHabitIcon"]').forEach(btn => {
        btn.classList.remove('bg-blue-100', 'ring-2', 'ring-blue-300');
    });
    
    // Highlight selected icon
    const selectedBtn = document.querySelector(`[onclick="selectHabitIcon('${iconValue}')"]`);
    if (selectedBtn) {
        selectedBtn.classList.add('bg-blue-100', 'ring-2', 'ring-blue-300');
    }
}

// Initialize default icon selection
document.addEventListener('DOMContentLoaded', function() {
    // Set default icon selection
    setTimeout(() => {
        selectHabitIcon('droplets');
    }, 100);
});

// Make selectHabitIcon global
window.selectHabitIcon = selectHabitIcon;
