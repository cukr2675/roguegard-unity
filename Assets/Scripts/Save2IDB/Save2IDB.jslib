const Save2IDBPlugin = {

    $Save2IDB: {
        importElements: {},

        export: (file) => {
            const element = document.createElement('a');
            element.style.display = 'none';
            element.download = file.name;
            element.href = URL.createObjectURL(file);
            document.body.appendChild(element);
            element.click();
            URL.revokeObjectURL(element.href);
            document.body.removeChild(element);
        },

        import: (filterAccept, multiselect, ohPtr) => {
            return new Promise((resolve) => {
                const element = document.createElement('input');
                element.style.display = 'none';
                element.type = 'file';
                element.accept = filterAccept;
                element.multiple = multiselect;
                element.addEventListener('change', () => {
                    resolve(Array.from(element.files));
                });
                element.addEventListener('cancel', () => {
                    resolve([]);
                });
                document.body.appendChild(element);
                element.click();
                Save2IDB.importElements[ohPtr] = element;
            });
        },

        disposeImporter: (ohPtr) => {
            const element = Save2IDB.importElements[ohPtr];
            if (!element) return;
            
            for (const file of element.files) {
                if (file.objectURL) { URL.revokeObjectURL(file.objectURL); } // For import to memory stream.
            }
            document.body.removeChild(element);
            delete Save2IDB.importElements[ohPtr];
        },

        getStatsJson: (files) => {
            const stats = files.map(({ name, destPath, size, type, lastModified, objectURL }) => ({ name, destPath, size, type, lastModified: new Date(lastModified), objectURL }));
            return JSON.stringify({ vs: stats }); // Convert to object because JsonUtility.FromJson cannot parse array.
        },
        
        callbackText: (callback, ohPtr, text) => {
            const buffer = new TextEncoder().encode(text + String.fromCharCode(0));
            const textPtr = Module._malloc(buffer.length);
            Module.HEAPU8.set(buffer, textPtr);
            Module.dynCall_vii(callback, ohPtr, textPtr);
            Module._free(textPtr);
        }
    },



    Save2IDB_ExportFrom: function (pathPtr, filenamePtr, contentTypePtr) {
        const path = UTF8ToString(pathPtr);
        const filename = UTF8ToString(filenamePtr);
        const contentType = UTF8ToString(contentTypePtr);

        const buffer = FS.readFile(path);
        const file = new File([buffer], filename, { type: contentType });
        Save2IDB.export(file);
    },

    Save2IDB_ExportAllBytes: function (bytesPtr, bytesLen, filenamePtr, contentTypePtr) {
        const buffer = Module.HEAPU8.subarray(bytesPtr, bytesPtr + bytesLen);
        const filename = UTF8ToString(filenamePtr);
        const contentType = UTF8ToString(contentTypePtr);

        const file = new File([buffer], filename, { type: contentType });
        Save2IDB.export(file);
    },

    // To file or Into directory
    Save2IDB_ImportToAsync: async function (pathPtr, overwrite, filterAcceptPtr, multiselect, ohPtr, thenCallback, catchCallback) {
        try {
            const path = UTF8ToString(pathPtr);
            const filterAccept = UTF8ToString(filterAcceptPtr);
            const pathIsDir = path.endsWith('/'); // If path ends with '/' then the path is a directory path, otherwise it is a file path.

            // Determine destination paths
            const files = await Save2IDB.import(filterAccept, multiselect);
            files.map(file => file.destPath = path + (pathIsDir ? file.name : ''));

            // Check overwrite
            if (!overwrite) {
                const existsFiles = files.filter(file => FS.analyzePath(file.destPath).exists);
                if (existsFiles.length >= 1) throw new Error(`Attempted to import ${existsFiles.map(file => file.destPath)}, but the file(s) already exists.`);
            }

            // Copy files
            for (const file of files) {
                const buffer = await file.arrayBuffer();
                FS.writeFile(file.destPath, new Uint8Array(buffer));
                FS.utime(file.destPath, file.lastModified, file.lastModified);
            }
            FS.syncfs(() => {});

            // Callback with file stats
            const statsJson = Save2IDB.getStatsJson(files);
            Save2IDB.callbackText(thenCallback, ohPtr, statsJson);

        } catch (error) {
            console.error(`Save2IDB_ImportToAsync error: ${error}`);
            Save2IDB.callbackText(catchCallback, ohPtr, error);
        }
    },

    // Dispose a file input element.
    Save2IDB_DisposeImporter: function (ohPtr) {
        Save2IDB.disposeImporter(ohPtr);
    },

    // To MemoryStreams
    Save2IDB_ImportToMemoryStreamsAsync: async function (filterAcceptPtr, multiselect, ohPtr, thenCallback, catchCallback) {
        try {
            const filterAccept = UTF8ToString(filterAcceptPtr);

            // Callback with file stats
            const files = await Save2IDB.import(filterAccept, multiselect, ohPtr);
            files.map(file => file.objectURL = URL.createObjectURL(file)); // For import to memory stream.
            const statsJson = Save2IDB.getStatsJson(files);
            Save2IDB.callbackText(thenCallback, ohPtr, statsJson);

        } catch (error) {
            console.error(`Save2IDB_ImportToMemoryStreamsAsync error: ${error}`);
            Save2IDB.callbackText(catchCallback, ohPtr, error);
        }
    }

};

autoAddDeps(Save2IDBPlugin, '$Save2IDB');
mergeInto(LibraryManager.library, Save2IDBPlugin);
