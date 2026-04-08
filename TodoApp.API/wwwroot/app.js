let mode = 'login';
let token = localStorage.getItem('todo_token');
let currentUser = localStorage.getItem('todo_user');

const authSection = document.getElementById('auth-section');
const todoSection = document.getElementById('todo-section');

let todosList = [];
let editingId = null;

// Init
if (token) {
    showTodoSection();
    loadTodos();
}

function switchAuthTab(newMode) {
    mode = newMode;
    document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));
    event.target.classList.add('active');
    
    const emailInput = document.getElementById('email');
    const submitBtn = document.getElementById('auth-submit');
    
    if (mode === 'register') {
        emailInput.style.display = 'block';
        emailInput.required = true;
        submitBtn.textContent = 'Register';
    } else {
        emailInput.style.display = 'none';
        emailInput.required = false;
        submitBtn.textContent = 'Login';
    }
}

async function handleAuth(e) {
    e.preventDefault();
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    const email = document.getElementById('email').value;
    const errorDiv = document.getElementById('auth-error');
    
    errorDiv.textContent = '';
    
    const endpoint = mode === 'login' ? '/api/Auth/login' : '/api/Auth/register';
    const payload = mode === 'login' ? { username, password } : { username, email, password };
    
    try {
        const response = await fetch(endpoint, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });
        
        const data = await response.json();
        
        if (!response.ok) throw new Error(data.message || 'Authentication failed');
        
        token = data.token;
        currentUser = data.username;
        localStorage.setItem('todo_token', token);
        localStorage.setItem('todo_user', currentUser);
        
        showTodoSection();
        loadTodos();
    } catch (err) {
        errorDiv.textContent = err.message;
    }
}

function showTodoSection() {
    authSection.style.display = 'none';
    todoSection.style.display = 'block';
    document.getElementById('user-display').textContent = currentUser;
}

function logout() {
    localStorage.removeItem('todo_token');
    localStorage.removeItem('todo_user');
    token = null;
    todoSection.style.display = 'none';
    authSection.style.display = 'block';
    document.getElementById('auth-form').reset();
}

const getHeaders = () => ({
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
});

async function loadTodos() {
    try {
        const response = await fetch('/api/Todo', { headers: getHeaders() });
        if (response.status === 401) return logout();
        todosList = await response.json();
        renderTodos();
    } catch (err) {
        console.error(err);
    }
}

function renderTodos() {
    const list = document.getElementById('todo-list');
    list.innerHTML = '';
    
    todosList.forEach(todo => {
        const item = document.createElement('div');
        item.className = 'todo-item';
        
        if (editingId === todo.id) {
            item.innerHTML = `
                <div class="todo-content" style="display:flex; flex-direction:column; gap:10px; width:100%;">
                    <input type="text" id="edit-title-${todo.id}" value="${todo.title.replace(/"/g, '&quot;')}" class="sub-input" style="background:rgba(0,0,0,0.3); border:1px solid var(--primary); padding:10px; width:100%; border-radius:8px; color:white;" />
                    <input type="text" id="edit-desc-${todo.id}" value="${todo.description ? todo.description.replace(/"/g, '&quot;') : ''}" placeholder="Details (optional)" class="sub-input" style="background:rgba(0,0,0,0.3); border:1px solid var(--primary); padding:10px; width:100%; border-radius:8px; color:white;" />
                    <div style="display:flex; gap:10px; margin-top:5px;">
                        <button class="btn-primary" style="padding:8px 15px; width:auto; font-size:0.8rem;" onclick="saveEdit(${todo.id}, ${todo.isCompleted})">Kaydet</button>
                        <button class="btn-icon" style="padding:8px 15px; width:auto; font-size:0.8rem; background:rgba(255,255,255,0.1); color:white;" onclick="cancelEdit()">İptal</button>
                    </div>
                </div>
            `;
        } else {
            const safeTitle = todo.title.replace(/'/g, "\\'").replace(/"/g, "&quot;");
            const safeDesc = todo.description ? todo.description.replace(/'/g, "\\'").replace(/"/g, "&quot;") : '';
            
            item.innerHTML = `
                <div class="todo-content">
                    <div class="todo-title">
                        <input type="checkbox" class="checkbox-custom" ${todo.isCompleted ? 'checked' : ''} 
                            onchange="toggleTodo(${todo.id}, '${safeTitle}', '${safeDesc}', this.checked)">
                        <span class="${todo.isCompleted ? 'completed' : ''}">${todo.title}</span>
                    </div>
                    ${todo.description ? `<div class="todo-desc ${todo.isCompleted ? 'completed' : ''}">${todo.description}</div>` : ''}
                </div>
                <div style="display:flex; gap:3px;">
                    <button class="edit-btn" onclick="startEdit(${todo.id})" title="Düzenle">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="color:var(--text-muted);"><path d="M12 20h9"></path><path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z"></path></svg>
                    </button>
                    <button class="delete-btn" onclick="deleteTodo(${todo.id})" title="Sil">
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>
                    </button>
                </div>
            `;
        }
        list.appendChild(item);
    });
}

function startEdit(id) {
    editingId = id;
    renderTodos();
}

function cancelEdit() {
    editingId = null;
    renderTodos();
}

async function saveEdit(id, isCompleted) {
    const title = document.getElementById(`edit-title-${id}`).value;
    const description = document.getElementById(`edit-desc-${id}`).value;
    
    if (!title.trim()) return;
    
    try {
        await fetch(`/api/Todo/${id}`, {
            method: 'PUT',
            headers: getHeaders(),
            body: JSON.stringify({ title, description, isCompleted })
        });
        editingId = null;
        loadTodos();
    } catch (err) {
        console.error(err);
    }
}

async function createTodo(e) {
    e.preventDefault();
    const title = document.getElementById('todo-title').value;
    const description = document.getElementById('todo-desc').value;
    
    try {
        const response = await fetch('/api/Todo', {
            method: 'POST',
            headers: getHeaders(),
            body: JSON.stringify({ title, description })
        });
        
        if (response.ok) {
            document.getElementById('todo-form').reset();
            loadTodos();
        }
    } catch (err) {
        console.error(err);
    }
}

async function toggleTodo(id, title, description, isCompleted) {
    try {
        await fetch(`/api/Todo/${id}`, {
            method: 'PUT',
            headers: getHeaders(),
            body: JSON.stringify({ title, description, isCompleted })
        });
        loadTodos();
    } catch (err) {
        console.error(err);
    }
}

async function deleteTodo(id) {
    try {
        await fetch(`/api/Todo/${id}`, {
            method: 'DELETE',
            headers: getHeaders()
        });
        loadTodos();
    } catch (err) {
        console.error(err);
    }
}
