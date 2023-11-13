const Save2IDBPlugin = {

  $Save2IDB: {
    databaseName: null,
    filesObjectStoreName: null,

    getDb: (mode, options) => {
      return new Promise((resolve, reject) => {
        if (Save2IDB.db) {
          resolve(Save2IDB.db);
        } else if (Save2IDB.databaseName) {
          const request = indexedDB.open(Save2IDB.databaseName);
          request.onerror = (event) => {
            reject(event.target);
          };
          request.onsuccess = (event) => {
            Save2IDB.db = event.target.result;
            resolve(Save2IDB.db);
          };
          request.onupgradeneeded = (event) => {
            const db = event.target.result;
            const objectStore = db.createObjectStore(Save2IDB.filesObjectStoreName, { keyPath: 'name' }); // File.name
            objectStore.createIndex('lastModified', 'lastModified', { unique: false }); // File.lastModified
          };
        } else {
          reject('Save2IDB error: Save2IDB has not been initialized.')
        }
      });
    },

    write: (path, buffer) => {
      return new Promise((resolve, reject) => {
        Save2IDB.getDb().then((db) => {
          const objectStore = db.transaction(Save2IDB.filesObjectStoreName, 'readwrite').objectStore(Save2IDB.filesObjectStoreName);
        
          // Set Date.now() to lastModified here.
          const file = new File([buffer], path);

          const request = objectStore.put(file);
          request.onsuccess = () => {
            resolve();
          };
          request.onerror = (event) => {
            reject(event.target);
          };

        }).catch((target) => {
          reject(target);
        });
      });
    },

    read: (path) => {
      return new Promise((resolve, reject) => {
        Save2IDB.getDb().then((db) => {
          const objectStore = db.transaction(Save2IDB.filesObjectStoreName).objectStore(Save2IDB.filesObjectStoreName);
        
          const request = objectStore.get(path);
          request.onsuccess = (event) => {
            const file = event.target.result;
            resolve(file);
          };
          request.onerror = (event) => {
            reject(event.target);
          };

        }).catch((target) => {
          reject(target);
        });
      });
    },

    delete: (path) => {
      return new Promise((resolve, reject) => {
        Save2IDB.getDb().then((db) => {
          const objectStore = db.transaction(Save2IDB.filesObjectStoreName, 'readwrite').objectStore(Save2IDB.filesObjectStoreName);
        
          const request = objectStore.delete(path);
          request.onsuccess = () => {
            resolve();
          };
          request.onerror = (event) => {
            reject(event.target);
          };

        }).catch((target) => {
          reject(target);
        });
      });
    },

    getFileInfosDescDate: () => {
      return new Promise((resolve, reject) => {
        Save2IDB.getDb().then((db) => {
          const objectStore = db.transaction(Save2IDB.filesObjectStoreName).objectStore(Save2IDB.filesObjectStoreName);
        
          let fileKeys = [];
          let request;
          if (/iPhone|iPad|iPod/i.test(navigator.userAgent)) {
            request = objectStore.openCursor();
          } else {
            const index = objectStore.index('lastModified'); // File.lastModified
            request = index.openKeyCursor(null, 'prev');
          }
          request.onsuccess = function(event) {
            const cursor = event.target.result;
            if (cursor) {
              const lastModified = cursor.value ? cursor.value.lastModified : cursor.key;
              let date = new Date();
              date.setTime(lastModified);
              fileKeys.push(`${cursor.primaryKey}|${date.toJSON()}`);
              cursor.continue();
            } else {
              resolve(fileKeys);
            }
          };
          request.onerror = (event) => {
            reject(event.target);
          };

        }).catch((target) => {
          reject(target);
        });
      });
    },

    callbackText: (callback, ohPtr, text) => {
      const buffer = new TextEncoder().encode(text + String.fromCharCode(0));
      const textPtr = Module._malloc(buffer.length);
      Module.HEAPU8.set(buffer, textPtr);
      Module.dynCall_vii(callback, ohPtr, textPtr);
      Module._free(textPtr);
    }

  },



  Save2IDB_Initialize: function (databaseNamePtr, filesObjectStoreNamePtr) {
    Save2IDB.databaseName = UTF8ToString(databaseNamePtr);
    Save2IDB.filesObjectStoreName = UTF8ToString(filesObjectStoreNamePtr);
  },
  
  Save2IDB_DirectWriteAsync: async function (ohPtr, pathPtr, bytesPtr, bytesLen, thenCallback, catchCallback) {
    try {
      const path = UTF8ToString(pathPtr);
      const bytesView = Module.HEAPU8.subarray(bytesPtr, bytesPtr + bytesLen);

      const buffer = new Uint8Array(bytesView).buffer; // Optimize buffer size.
      await Save2IDB.write(path, buffer);
      Module.dynCall_vi(thenCallback, ohPtr);

    } catch (error) {
      console.error(`Save2IDB_DirectWriteAsync error: ${error}`);
      Save2IDB.callbackText(catchCallback, ohPtr, error);
    }
  },

  Save2IDB_OpenReadAsync: async function (ohPtr, pathPtr, thenCallback, catchCallback) {
    try {
      const path = UTF8ToString(pathPtr);

      const file = await Save2IDB.read(path);
      const buffer = await file.arrayBuffer();
      const bytesPtr = Module._malloc(buffer.byteLength);
      Module.HEAPU8.set(new Uint8Array(buffer), bytesPtr);
      Module.dynCall_viij(thenCallback, ohPtr, bytesPtr, buffer.byteLength);

    } catch (error) {
      console.error(`Save2IDB_OpenReadAsync error: ${error}`);
      Save2IDB.callbackText(catchCallback, ohPtr, error);
    }
  },

  Save2IDB_CloseRead: function (ptr) {
    Module._free(ptr);
  },

  Save2IDB_DeleteAsync: async function (ohPtr, pathPtr, thenCallback, catchCallback) {
    try {
      const path = UTF8ToString(pathPtr);
      await Save2IDB.delete(path);
      Module.dynCall_vi(thenCallback, ohPtr);
    } catch (error) {
      console.error(`Save2IDB_DeleteAsync error: ${error}`);
      Save2IDB.callbackText(catchCallback, ohPtr, error);
    }
  },

  Save2IDB_GetFileInfosDescDateAsync: async function (ohPtr, thenCallback, catchCallback) {
    try {
      const fileKeys = await Save2IDB.getFileInfosDescDate();
      const serial = fileKeys.join('|');
      Save2IDB.callbackText(thenCallback, ohPtr, serial);
    } catch (error) {
      console.error(`Save2IDB_GetFileInfosDescDateAsync error: ${error}`);
      Save2IDB.callbackText(catchCallback, ohPtr, error);
    }
  }

};

autoAddDeps(Save2IDBPlugin, '$Save2IDB');
mergeInto(LibraryManager.library, Save2IDBPlugin);
