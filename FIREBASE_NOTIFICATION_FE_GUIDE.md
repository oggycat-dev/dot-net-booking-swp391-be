# 🔔 Firebase Notification - Frontend Implementation Guide

## 📋 Overview

Backend đã implement Firebase Cloud Messaging để gửi notification realtime cho Admin khi:
- ✅ Có đơn đăng ký mới (Student/Lecturer register)
- ✅ Có yêu cầu đổi campus
- ✅ Lecturer tạo booking mới

## 🚀 Frontend (Next.js) Implementation

### **1. Install Dependencies**

```bash
npm install firebase
# hoặc
yarn add firebase
```

### **2. Lấy Firebase Configuration từ Console**

1. Truy cập [Firebase Console](https://console.firebase.google.com/)
2. Chọn project của bạn
3. Vào **Project Settings** (⚙️) > **General**
4. Scroll xuống **Your apps** > Chọn Web app hoặc click **Add app** nếu chưa có
5. Copy **firebaseConfig** object
6. Vào tab **Cloud Messaging** > Copy **VAPID key** (Web Push certificates)

### **3. Tạo Firebase Config File**

```typescript
// src/lib/firebase.ts
import { initializeApp, getApps } from 'firebase/app';
import { getMessaging, getToken, onMessage, Messaging } from 'firebase/messaging';

const firebaseConfig = {
  apiKey: process.env.NEXT_PUBLIC_FIREBASE_API_KEY,
  authDomain: process.env.NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN,
  projectId: process.env.NEXT_PUBLIC_FIREBASE_PROJECT_ID,
  storageBucket: process.env.NEXT_PUBLIC_FIREBASE_STORAGE_BUCKET,
  messagingSenderId: process.env.NEXT_PUBLIC_FIREBASE_MESSAGING_SENDER_ID,
  appId: process.env.NEXT_PUBLIC_FIREBASE_APP_ID
};

// Initialize Firebase
const app = getApps().length === 0 ? initializeApp(firebaseConfig) : getApps()[0];

// Initialize Firebase Cloud Messaging
let messaging: Messaging | null = null;
if (typeof window !== 'undefined') {
  messaging = getMessaging(app);
}

export { messaging, getToken, onMessage };
```

### **4. Tạo Environment Variables**

Tạo file `.env.local`:

```env
NEXT_PUBLIC_FIREBASE_API_KEY=your-api-key
NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN=your-project.firebaseapp.com
NEXT_PUBLIC_FIREBASE_PROJECT_ID=your-project-id
NEXT_PUBLIC_FIREBASE_STORAGE_BUCKET=your-project.appspot.com
NEXT_PUBLIC_FIREBASE_MESSAGING_SENDER_ID=your-sender-id
NEXT_PUBLIC_FIREBASE_APP_ID=your-app-id
NEXT_PUBLIC_FIREBASE_VAPID_KEY=your-vapid-key
```

### **5. Tạo Service Worker**

Tạo file `public/firebase-messaging-sw.js`:

```javascript
// public/firebase-messaging-sw.js
importScripts('https://www.gstatic.com/firebasejs/10.7.1/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/10.7.1/firebase-messaging-compat.js');

firebase.initializeApp({
  apiKey: "your-api-key",
  authDomain: "your-project.firebaseapp.com",
  projectId: "your-project-id",
  storageBucket: "your-project.appspot.com",
  messagingSenderId: "your-sender-id",
  appId: "your-app-id"
});

const messaging = firebase.messaging();

// Handle background messages
messaging.onBackgroundMessage((payload) => {
  console.log('[firebase-messaging-sw.js] Received background message', payload);
  
  const notificationTitle = payload.notification?.title || 'New Notification';
  const notificationOptions = {
    body: payload.notification?.body || '',
    icon: '/logo.png',
    badge: '/badge.png',
    tag: payload.data?.type || 'notification',
    data: payload.data,
    requireInteraction: true
  };

  return self.registration.showNotification(notificationTitle, notificationOptions);
});

// Handle notification click
self.addEventListener('notificationclick', (event) => {
  console.log('[firebase-messaging-sw.js] Notification click received.', event);
  
  event.notification.close();
  
  const data = event.notification.data;
  let url = '/admin/dashboard';
  
  // Navigate based on notification type
  if (data.type === 'new_registration') {
    url = '/admin/registrations';
  } else if (data.type === 'campus_change_request') {
    url = '/admin/campus-changes';
  } else if (data.type === 'new_booking') {
    url = `/admin/bookings/${data.bookingId}`;
  }
  
  event.waitUntil(
    clients.openWindow(url)
  );
});
```

### **6. Tạo Notification Hook**

```typescript
// src/hooks/useFirebaseNotification.ts
import { useEffect, useState } from 'react';
import { messaging, getToken, onMessage } from '@/lib/firebase';
import { toast } from 'react-hot-toast'; // hoặc notification library bạn dùng
import { useRouter } from 'next/navigation';

interface NotificationPayload {
  notification?: {
    title: string;
    body: string;
  };
  data?: {
    type: string;
    bookingId?: string;
    userId?: string;
    requestId?: string;
    [key: string]: string | undefined;
  };
}

export function useFirebaseNotification() {
  const [fcmToken, setFcmToken] = useState<string | null>(null);
  const [isSupported, setIsSupported] = useState(false);
  const router = useRouter();

  useEffect(() => {
    // Check if browser supports notifications
    if (typeof window !== 'undefined' && 'Notification' in window && messaging) {
      setIsSupported(true);
    }
  }, []);

  const requestPermission = async () => {
    if (!isSupported || !messaging) {
      console.log('Notifications not supported');
      return null;
    }

    try {
      const permission = await Notification.requestPermission();
      
      if (permission === 'granted') {
        console.log('Notification permission granted');
        
        // Get FCM token
        const token = await getToken(messaging, {
          vapidKey: process.env.NEXT_PUBLIC_FIREBASE_VAPID_KEY
        });
        
        if (token) {
          console.log('FCM Token:', token);
          setFcmToken(token);
          return token;
        } else {
          console.log('No registration token available');
          return null;
        }
      } else {
        console.log('Notification permission denied');
        return null;
      }
    } catch (error) {
      console.error('Error getting FCM token:', error);
      return null;
    }
  };

  const registerToken = async (token: string, authToken: string) => {
    try {
      const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/notifications/register-token`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${authToken}`
        },
        body: JSON.stringify({ fcmToken: token })
      });

      if (response.ok) {
        console.log('FCM token registered successfully');
        toast.success('Notifications enabled!');
        return true;
      } else {
        console.error('Failed to register FCM token');
        return false;
      }
    } catch (error) {
      console.error('Error registering FCM token:', error);
      return false;
    }
  };

  const unregisterToken = async (authToken: string) => {
    try {
      const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/notifications/unregister-token`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${authToken}`
        }
      });

      if (response.ok) {
        console.log('FCM token unregistered successfully');
        setFcmToken(null);
        return true;
      }
      return false;
    } catch (error) {
      console.error('Error unregistering FCM token:', error);
      return false;
    }
  };

  useEffect(() => {
    if (!messaging) return;

    // Listen for foreground messages
    const unsubscribe = onMessage(messaging, (payload: NotificationPayload) => {
      console.log('Foreground message received:', payload);
      
      const title = payload.notification?.title || 'New Notification';
      const body = payload.notification?.body || '';
      const data = payload.data;

      // Show toast notification
      toast(
        (t) => (
          <div className="flex flex-col gap-2">
            <div className="font-semibold">{title}</div>
            <div className="text-sm text-gray-600">{body}</div>
            <button
              onClick={() => {
                toast.dismiss(t.id);
                handleNotificationClick(data);
              }}
              className="text-sm text-blue-600 hover:underline text-left"
            >
              View Details
            </button>
          </div>
        ),
        {
          duration: 5000,
          icon: '🔔'
        }
      );
    });

    return () => {
      unsubscribe();
    };
  }, []);

  const handleNotificationClick = (data?: NotificationPayload['data']) => {
    if (!data) return;

    switch (data.type) {
      case 'new_registration':
        router.push('/admin/registrations');
        break;
      case 'campus_change_request':
        router.push('/admin/campus-changes');
        break;
      case 'new_booking':
        if (data.bookingId) {
          router.push(`/admin/bookings/${data.bookingId}`);
        }
        break;
      default:
        router.push('/admin/dashboard');
    }
  };

  return {
    fcmToken,
    isSupported,
    requestPermission,
    registerToken,
    unregisterToken
  };
}
```

### **7. Sử dụng trong Admin Layout/Component**

```typescript
// src/app/admin/layout.tsx hoặc components/AdminLayout.tsx
'use client';

import { useEffect } from 'react';
import { useFirebaseNotification } from '@/hooks/useFirebaseNotification';
import { useAuth } from '@/hooks/useAuth'; // Hook lấy auth token của bạn

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const { user, token } = useAuth(); // Lấy admin token
  const { isSupported, requestPermission, registerToken } = useFirebaseNotification();

  useEffect(() => {
    const setupNotifications = async () => {
      // Chỉ setup cho Admin
      if (user?.role === 'Admin' && token && isSupported) {
        // Request permission và lấy FCM token
        const fcmToken = await requestPermission();
        
        if (fcmToken) {
          // Register token với backend
          await registerToken(fcmToken, token);
        }
      }
    };

    setupNotifications();
  }, [user, token, isSupported]);

  return (
    <div className="admin-layout">
      <Sidebar />
      <main>{children}</main>
    </div>
  );
}
```

### **8. Testing**

#### **Test 1: Đăng ký FCM Token**

1. Login với admin account
2. Cho phép notifications khi browser hỏi
3. Check console log để thấy FCM token
4. Verify token đã được lưu vào backend

#### **Test 2: Nhận Notification**

**Từ Backend:**
```bash
# Tạo đơn đăng ký mới
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test123@fpt.edu.vn",
    "password": "Test@123456",
    "confirmPassword": "Test@123456",
    "fullName": "Test User",
    "phoneNumber": "0123456789",
    "campusId": "a51b504a-2001-49ed-910a-599b37696703",
    "role": "Student",
    "department": "IT"
  }'
```

**Hoặc từ Firebase Console:**
1. Vào Firebase Console > Cloud Messaging
2. Click "Send test message"
3. Paste FCM token của admin
4. Nhập title và body
5. Click "Test"

→ Admin web sẽ nhận notification ngay lập tức! 🔔

## 📊 Backend API Endpoints

### **POST /api/notifications/register-token**
Register FCM token cho admin user

**Headers:**
```
Authorization: Bearer {admin-jwt-token}
Content-Type: application/json
```

**Body:**
```json
{
  "fcmToken": "string"
}
```

**Response:**
```json
{
  "statusCode": 200,
  "success": true,
  "message": "FCM token registered successfully"
}
```

### **DELETE /api/notifications/unregister-token**
Unregister FCM token (khi logout)

**Headers:**
```
Authorization: Bearer {admin-jwt-token}
```

**Response:**
```json
{
  "statusCode": 200,
  "success": true,
  "message": "FCM token unregistered successfully"
}
```

## 🎯 Notification Data Structure

Backend gửi notification với structure:

```typescript
{
  notification: {
    title: string,
    body: string
  },
  data: {
    type: "new_registration" | "campus_change_request" | "new_booking",
    userId?: string,
    userName?: string,
    userEmail?: string,
    userRole?: string,
    campusId?: string,
    campusName?: string,
    bookingId?: string,
    facilityName?: string,
    bookingDate?: string,
    startTime?: string,
    endTime?: string,
    requestId?: string,
    requestedCampusId?: string,
    requestedCampusName?: string
  }
}
```

## 🔍 Debugging

### **Check if FCM token is registered:**

```sql
-- Trong database
SELECT "Id", "Email", "FcmToken", "Role" 
FROM "Users" 
WHERE "Role" = 'Admin' AND "FcmToken" IS NOT NULL;
```

### **Check backend logs:**

```bash
docker logs clean-architecture-api --tail 50 | grep -i "firebase\|notification\|fcm"
```

### **Test notification manually:**

```bash
# Với token đã register
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@fpt.edu.vn","password":"Test@123",...}'

# Check logs
docker logs clean-architecture-api --tail 20
```

## 📝 Important Notes

1. **VAPID Key**: Phải lấy từ Firebase Console > Cloud Messaging > Web Push certificates
2. **Service Worker**: File `firebase-messaging-sw.js` phải ở `public/` folder
3. **HTTPS Required**: Firebase messaging chỉ hoạt động trên HTTPS (hoặc localhost)
4. **Browser Support**: Check `'Notification' in window` trước khi request permission
5. **Admin Only**: Chỉ Admin mới có thể register FCM token

## 🚀 Next Steps

1. ✅ Implement phần FE theo hướng dẫn trên
2. ✅ Test với Firebase Console trước
3. ✅ Integrate vào admin dashboard
4. ✅ Thêm notification center/bell icon để hiển thị history
5. ✅ Customize notification UI/UX theo design của bạn

## 🔗 References

- [Firebase Web Setup](https://firebase.google.com/docs/web/setup)
- [Firebase Cloud Messaging for Web](https://firebase.google.com/docs/cloud-messaging/js/client)
- [Next.js with Firebase](https://firebase.google.com/docs/web/nextjs)

---

**Backend Status:** ✅ Hoàn tất và sẵn sàng  
**Frontend Required:** 🔨 Cần implement theo guide này

Good luck! 🎉
